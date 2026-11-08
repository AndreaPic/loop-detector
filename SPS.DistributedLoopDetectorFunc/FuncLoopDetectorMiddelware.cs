using DistributedLoopDetector;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace SPS.DistributedLoopDetectorFunc
{
    /// <summary>Detects distributed loops for HTTP-triggered functions.</summary>
    public class FuncLoopDetectorMiddelware : IFunctionsWorkerMiddleware
    {
        private readonly LoopDetectorHandler handler;
        private readonly LoopDetectStackInstance loopDetectStack;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AcceptedLoop acceptedLoop;
        private readonly ILogger<FuncLoopDetectorMiddelware> logger;

        public FuncLoopDetectorMiddelware(LoopDetectorHandler loopDetectorHandler,
            LoopDetectStackInstance loopDetectStackInstance, IHttpContextAccessor httpContextAccessor)
            : this(loopDetectorHandler, loopDetectStackInstance, httpContextAccessor, new LoopDetectorOptions(),
                NullLogger<FuncLoopDetectorMiddelware>.Instance)
        {
        }

        [ActivatorUtilitiesConstructor]
        public FuncLoopDetectorMiddelware(LoopDetectorHandler loopDetectorHandler,
            LoopDetectStackInstance loopDetectStackInstance, IHttpContextAccessor httpContextAccessor,
            LoopDetectorOptions options, ILogger<FuncLoopDetectorMiddelware> logger)
        {
            handler = loopDetectorHandler;
            loopDetectStack = loopDetectStackInstance;
            this.httpContextAccessor = httpContextAccessor;
            acceptedLoop = options.AcceptedLoop;
            this.logger = logger;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var request = await context.GetHttpRequestDataAsync();
            if (request == null)
            {
                await next(context);
                return;
            }

            string path = context.FunctionDefinition.Name;
            var previousContext = httpContextAccessor.HttpContext;
            httpContextAccessor.HttpContext = new DefaultHttpContext { Items = (IDictionary<object, object?>)context.Items };
            string? registration = null;
            string[] loopIds = Array.Empty<string>();
            bool timedOut = false;
            FunctionRequestBuffer? buffer = null;
            try
            {
                var fingerprint = new LoopRequestFingerprint(null, null);
                if (acceptedLoop != AcceptedLoop.None)
                {
                    var comparisonRequest = new DefaultHttpContext().Request;
                    comparisonRequest.Path = request.Url.AbsolutePath;
                    comparisonRequest.QueryString = new QueryString(request.Url.Query);
                    comparisonRequest.Body = request.Body;
                    foreach (var header in request.Headers)
                        comparisonRequest.Headers[header.Key] = header.Value.ToArray();
                    // Route binding data are request parameters. Exclude worker metadata and other inputs.
                    foreach (var binding in context.BindingContext.BindingData)
                    {
                        if (!string.Equals(binding.Key, "Headers", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(binding.Key, "Query", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(binding.Key, "sys", StringComparison.OrdinalIgnoreCase)
                            && !context.FunctionDefinition.InputBindings.ContainsKey(binding.Key))
                            comparisonRequest.RouteValues[binding.Key] = binding.Value;
                    }
                    if (acceptedLoop.HasFlag(AcceptedLoop.WithDifferentPayload))
                        buffer = FunctionRequestBuffer.Create(context, request, comparisonRequest);
                    fingerprint = await LoopRequestFingerprint.CreateAsync(comparisonRequest, acceptedLoop, context.CancellationToken);
                }

                loopIds = request.Headers.TryGetValues(LoopDetectorHandler.HeaderName, out var values)
                    ? values.Where(id => !string.IsNullOrEmpty(id)).Distinct(StringComparer.Ordinal).ToArray()
                    : Array.Empty<string>();
                if (loopIds.Length == 0)
                    loopIds = new[] { Guid.NewGuid().ToString("N") };
                registration = loopDetectStack.TryRegisterRequest(path, loopIds, fingerprint, acceptedLoop, out bool excluded);
                if (registration == null)
                {
                    logger.LogError("Distributed Loop Detected in: {Api}", path);
                    context.GetInvocationResult().Value = request.CreateResponse(HttpStatusCode.LoopDetected);
                    return;
                }
                if (excluded)
                    logger.LogWarning("Distributed loop detected in {Api} for {LoopIds}, excluded by {AcceptedLoop} because the selected request fields differ.",
                        path, string.Join(",", loopIds), acceptedLoop);

                if (!request.Headers.Contains(LoopDetectorHandler.HeaderName))
                    request.Headers.Add(LoopDetectorHandler.HeaderName, loopIds);
                context.Items[LoopDetectorHandler.HeaderName] = loopIds;
                handler.SetFunctionContext(context);
                await next(context);
            }
            catch (Exception exception) when (exception is TimeoutException || exception is TaskCanceledException)
            {
                timedOut = true;
                throw;
            }
            finally
            {
                if (registration != null)
                {
                    if (!timedOut && !context.Items.ContainsKey(HttpStatusCode.RequestTimeout.ToString()))
                        loopDetectStack.RemoveRequest(path, loopIds, registration);
                    context.Items.Remove(LoopDetectorHandler.HeaderName);
                }
                buffer?.Dispose();
                httpContextAccessor.HttpContext = previousContext;
            }
        }
    }
}
