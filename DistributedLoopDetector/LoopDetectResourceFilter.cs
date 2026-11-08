using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedLoopDetector
{
    /// <summary>
    /// This resourcefilter return http error if Loop is detected
    /// </summary>
    public class LoopDetectResourceFilter : IResourceFilter, IAsyncResourceFilter
    {

        private ILogger _logger;
        private readonly AcceptedLoop acceptedLoop;
        internal const string RegistrationKey = "DistributedLoopDetector.RequestRegistration";
        //TODO: use DI constructor for Logger
        public LoopDetectResourceFilter(ILogger<LoopDetectResourceFilter> logger)
            : this(logger, new LoopDetectorOptions())
        {
        }

        [ActivatorUtilitiesConstructor]
        public LoopDetectResourceFilter(ILogger<LoopDetectResourceFilter> logger, LoopDetectorOptions options)
        {
            _logger = logger;
            acceptedLoop = options.AcceptedLoop;
        }

        /// <summary>Compare requests before model binding, preserving the body for the action.</summary>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (acceptedLoop == AcceptedLoop.None)
            {
                OnResourceExecuting(context);
                if (context.Result == null)
                    await next();
                return;
            }

            var request = context.HttpContext.Request;
            string path = request.Path;
            if (string.IsNullOrEmpty(path))
            {
                await next();
                return;
            }
            var fingerprint = await LoopRequestFingerprint.CreateAsync(request, acceptedLoop, context.HttpContext.RequestAborted);
            var loopIds = request.Headers[LoopDetectorHandler.HeaderName].Where(id => !string.IsNullOrEmpty(id)).Cast<string>().ToArray();
            if (loopIds.Length == 0)
                loopIds = new[] { Guid.NewGuid().ToString("N") };

            var stack = LoopDetectStack.Instance;
            var registration = stack.TryRegisterRequest(path, loopIds, fingerprint, acceptedLoop, out bool excluded);
            if (registration == null)
            {
                _logger.LogError("Distributed Loop Detected in: {Api}", context.ActionDescriptor.DisplayName);
                context.Result = new StatusCodeResult(508);
                return;
            }
            if (excluded)
                _logger.LogWarning("Distributed loop detected in {Api} for {LoopIds}, excluded by {AcceptedLoop} because the selected request fields differ.",
                    context.ActionDescriptor.DisplayName, string.Join(",", loopIds), acceptedLoop);

            request.Headers[LoopDetectorHandler.HeaderName] = new StringValues(loopIds);
            context.HttpContext.Items[LoopDetectorHandler.HeaderName] = loopIds;
            context.HttpContext.Items[RegistrationKey] = registration;
            bool timedOut = false;
            try
            {
                var executed = await next();
                timedOut = executed.Exception is TimeoutException || executed.Exception is TaskCanceledException;
            }
            catch (Exception exception) when (exception is TimeoutException || exception is TaskCanceledException)
            {
                timedOut = true;
                throw;
            }
            finally
            {
                if (!timedOut && !context.HttpContext.Items.ContainsKey(HttpStatusCode.RequestTimeout.ToString()))
                    stack.RemoveRequest(path, loopIds, registration);
                context.HttpContext.Items.Remove(RegistrationKey);
                context.HttpContext.Items.Remove(LoopDetectorHandler.HeaderName);
            }
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        /// <param name="context">Executed context</param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
        /// <summary>
        /// If there is a current loop id context inside the request's header its return http error
        /// </summary>
        /// <param name="context">Executing context</param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context is not null)
            {
                var headers = context!.HttpContext?.Request?.Headers;
                string? path = context.HttpContext?.Request?.Path;
                if ((headers != null) && (!string.IsNullOrEmpty(path)))
                {
                    var header = headers.FirstOrDefault(head => head.Key == LoopDetectorHandler.HeaderName);                    
                    bool loopDetected = header.Value.Any(stringValue => stringValue != null && LoopDetectStack.Instance.LoopDetectInfoMatch(path, stringValue));
                    if (loopDetected)
                    {
                        if (context?.HttpContext?.RequestServices != null)
                        {
                            //var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger)) as ILogger;
                            if (_logger != null)
                            {
                                //TODO: use resources
                                _logger.Log(LogLevel.Error, $"Distributed Loop Detected in: { context?.ActionDescriptor?.DisplayName }");
                            }
                        }

                        context!.Result = new StatusCodeResult(508);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Warning, $"Loop Not Foud for {header.Value}");
                    }
                }
            }            
        }
    }
}
