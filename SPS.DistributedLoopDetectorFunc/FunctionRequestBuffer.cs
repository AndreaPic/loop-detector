using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Context.Features;
using Microsoft.Azure.Functions.Worker.Http;
using System.Security.Claims;

namespace SPS.DistributedLoopDetectorFunc
{
    // HttpRequestData.Body has no setter. Replace the worker's request and input features so
    // both GetHttpRequestDataAsync and the function's HttpRequestData parameter see the buffer.
    internal sealed class FunctionRequestBuffer : IDisposable, IHttpRequestDataFeature, IFunctionInputBindingFeature
    {
        private readonly FunctionContext context;
        private readonly HttpRequestData buffered;
        private readonly IHttpRequestDataFeature? requestFeature;
        private readonly IFunctionInputBindingFeature? inputFeature;
        private readonly List<(object?[] Values, int Index, object? Original)> replacedInputs = new();

        private FunctionRequestBuffer(FunctionContext context, HttpRequestData original, Stream body)
        {
            this.context = context;
            buffered = new BufferedRequest(original, body);
            requestFeature = context.Features.Get<IHttpRequestDataFeature>();
            inputFeature = context.Features.Get<IFunctionInputBindingFeature>();
            context.Features.Set<IHttpRequestDataFeature>(this);
            if (inputFeature != null)
                context.Features.Set<IFunctionInputBindingFeature>(this);
        }

        internal static FunctionRequestBuffer? Create(FunctionContext context, HttpRequestData request, HttpRequest comparisonRequest)
        {
            comparisonRequest.EnableBuffering();
            return ReferenceEquals(comparisonRequest.Body, request.Body)
                ? null : new FunctionRequestBuffer(context, request, comparisonRequest.Body);
        }

        public ValueTask<HttpRequestData?> GetHttpRequestDataAsync(FunctionContext context) => new(buffered);

        public async ValueTask<FunctionInputBindingResult> BindFunctionInputAsync(FunctionContext context)
        {
            var result = await inputFeature!.BindFunctionInputAsync(context);
            for (int i = 0; i < result.Values.Length; i++)
            {
                // A converter can produce another HttpRequestData instance for the same trigger.
                if (result.Values[i] is HttpRequestData && !ReferenceEquals(result.Values[i], buffered))
                {
                    replacedInputs.Add((result.Values, i, result.Values[i]));
                    result.Values[i] = buffered;
                }
            }
            return result;
        }

        public void Dispose()
        {
            foreach (var input in replacedInputs)
                input.Values[input.Index] = input.Original;
            context.Features.Set(requestFeature!);
            if (inputFeature != null)
                context.Features.Set(inputFeature);
            buffered.Body.Dispose();
        }

        private sealed class BufferedRequest : HttpRequestData
        {
            private readonly HttpRequestData original;
            public BufferedRequest(HttpRequestData original, Stream body) : base(original.FunctionContext)
            {
                this.original = original;
                Body = body;
            }
            public override Stream Body { get; }
            public override HttpHeadersCollection Headers => original.Headers;
            public override IReadOnlyCollection<IHttpCookie> Cookies => original.Cookies;
            public override Uri Url => original.Url;
            public override IEnumerable<ClaimsIdentity> Identities => original.Identities;
            public override string Method => original.Method;
            public override HttpResponseData CreateResponse() => original.CreateResponse();
        }
    }
}
