extern alias Functions;

using FunctionPolicy = Functions::DistributedLoopDetector.AcceptedLoop;
using FunctionServices = Functions::SPS.DistributedLoopDetectorFunc.FuncServiceExtensions;
using FunctionMiddleware = Functions::SPS.DistributedLoopDetectorFunc.FuncLoopDetectorMiddelware;
using FunctionOptions = Functions::DistributedLoopDetector.LoopDetectorOptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Context.Features;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DistributedLoopDetectorTests
{
    public class FunctionAcceptedLoopTests
    {
        [Theory]
        [InlineData(FunctionPolicy.None, "body", false)]
        [InlineData(FunctionPolicy.None, "query", false)]
        [InlineData(FunctionPolicy.None, "headers", false)]
        [InlineData(FunctionPolicy.WithDifferentPayload, "body", true)]
        [InlineData(FunctionPolicy.WithDifferentPayload, "query", true)]
        [InlineData(FunctionPolicy.WithDifferentPayload, "route", true)]
        [InlineData(FunctionPolicy.WithDifferentPayload, "headers", false)]
        [InlineData(FunctionPolicy.WithDifferentHeaders, "body", false)]
        [InlineData(FunctionPolicy.WithDifferentHeaders, "query", false)]
        [InlineData(FunctionPolicy.WithDifferentHeaders, "headers", true)]
        [InlineData(FunctionPolicy.WithDifferentHeaders, "tracing", false)]
        [InlineData(FunctionPolicy.WithDifferentPayload | FunctionPolicy.WithDifferentHeaders, "body", true)]
        [InlineData(FunctionPolicy.WithDifferentPayload | FunctionPolicy.WithDifferentHeaders, "headers", true)]
        public async Task FunctionMiddlewareUsesTheSelectedPolicy(FunctionPolicy policy, string difference, bool allowed)
        {
            var logs = new CapturedLogs();
            using var provider = Services(policy, logs);
            var middleware = ActivatorUtilities.CreateInstance<FunctionMiddleware>(provider);
            var first = new FakeFunctionContext(provider, "a", "?id=1", "a");
            var second = new FakeFunctionContext(provider, difference == "body" ? "b" : "a",
                difference == "query" ? "?id=2" : "?id=1", difference == "headers" ? "b" : "a");
            if (difference == "route") second.Bindings.Data["routeId"] = "2";
            if (difference == "tracing") second.Request.Headers.Add("traceparent", "new-trace");
            bool reached = false;
            await middleware.Invoke(first, async outer =>
            {
                CopyLoopId(outer, second);
                await middleware.Invoke(second, _ => { reached = true; return Task.CompletedTask; });
            });
            Assert.Equal(allowed, reached);
            if (allowed)
            {
                Assert.Contains(logs.Messages, log => log.Level == LogLevel.Warning && log.Message.Contains("excluded by"));
                Assert.Null(second.GetInvocationResult().Value);
            }
            else
                Assert.Equal(HttpStatusCode.LoopDetected, Assert.IsType<FakeResponse>(second.GetInvocationResult().Value).StatusCode);

            // A completed invocation no longer blocks the same loop ID.
            await middleware.Invoke(second, _ => { reached = true; return Task.CompletedTask; });
            Assert.True(reached);
        }

        [Theory]
        [InlineData(FunctionPolicy.WithDifferentPayload)]
        [InlineData(FunctionPolicy.WithDifferentHeaders)]
        [InlineData(FunctionPolicy.WithDifferentPayload | FunctionPolicy.WithDifferentHeaders)]
        public async Task AnEarlierActiveVariationIsStillBlocked(FunctionPolicy policy)
        {
            using var provider = Services(policy, new CapturedLogs());
            var middleware = ActivatorUtilities.CreateInstance<FunctionMiddleware>(provider);
            var first = new FakeFunctionContext(provider, "a", "?id=1", "a");
            var second = new FakeFunctionContext(provider, "b", "?id=1", "b");
            var third = new FakeFunctionContext(provider, "a", "?id=1", "a");
            await middleware.Invoke(first, async outer =>
            {
                CopyLoopId(outer, second);
                await middleware.Invoke(second, async middle =>
                {
                    CopyLoopId(middle, third);
                    await middleware.Invoke(third, _ => throw new Exception("Repeated request must be blocked."));
                });
            });
            Assert.Equal(HttpStatusCode.LoopDetected, Assert.IsType<FakeResponse>(third.GetInvocationResult().Value).StatusCode);
        }

        [Fact]
        public async Task NonSeekableBodyRemainsAvailableThroughBothWorkerFeatures()
        {
            using var provider = Services(FunctionPolicy.WithDifferentPayload, new CapturedLogs());
            var middleware = ActivatorUtilities.CreateInstance<FunctionMiddleware>(provider);
            var context = new FakeFunctionContext(provider, "body", nonSeekable: true);
            await middleware.Invoke(context, async current =>
            {
                var request = await current.GetHttpRequestDataAsync();
                Assert.True(request.Body.CanSeek);
                Assert.Equal("body", await new StreamReader(request.Body, leaveOpen: true).ReadToEndAsync());
                request.Body.Position = 0;
                var input = await current.Features.Get<IFunctionInputBindingFeature>().BindFunctionInputAsync(current);
                var parameter = Assert.IsAssignableFrom<HttpRequestData>(input.Values[0]);
                Assert.Same(request, parameter);
                Assert.Equal("body", await new StreamReader(parameter.Body, leaveOpen: true).ReadToEndAsync());
            });
            Assert.Same(context.Request, await context.GetHttpRequestDataAsync());
            var restoredInput = await context.Features.Get<IFunctionInputBindingFeature>().BindFunctionInputAsync(context);
            Assert.Same(context.Request, restoredInput.Values[0]);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task CleanupRemovesFailuresButRetainsTimeoutProtection(bool timeout)
        {
            using var provider = Services(FunctionPolicy.WithDifferentPayload, new CapturedLogs());
            var middleware = ActivatorUtilities.CreateInstance<FunctionMiddleware>(provider);
            var first = new FakeFunctionContext(provider);
            first.Request.Headers.Add("X-LOOP-DETECT", "loop");
            await Assert.ThrowsAnyAsync<Exception>(() => middleware.Invoke(first,
                _ => throw (timeout ? new TimeoutException() : new InvalidOperationException())));
            var again = new FakeFunctionContext(provider);
            again.Request.Headers.Add("X-LOOP-DETECT", "loop");
            bool reached = false;
            await middleware.Invoke(again, _ => { reached = true; return Task.CompletedTask; });
            Assert.Equal(!timeout, reached);
            if (timeout)
                Assert.Equal(HttpStatusCode.LoopDetected, Assert.IsType<FakeResponse>(again.GetInvocationResult().Value).StatusCode);
        }

        [Fact]
        public async Task PooledHttpClientHandlerUsesEachCurrentInvocation()
        {
            var outgoing = new List<string>();
            var services = new ServiceCollection();
            FunctionServices.AddFuncDistributedLoopDetector(services, FunctionPolicy.WithDifferentHeaders);
            services.AddHttpClient("capture").ConfigurePrimaryHttpMessageHandler(() => new CaptureHandler(outgoing));
            using var provider = services.BuildServiceProvider();
            var middleware = ActivatorUtilities.CreateInstance<FunctionMiddleware>(provider);
            foreach (var id in new[] { "first", "second" })
            {
                var context = new FakeFunctionContext(provider);
                context.Request.Headers.Add("X-LOOP-DETECT", id);
                await middleware.Invoke(context, async _ =>
                {
                    using var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient("capture");
                    using var response = await client.GetAsync("http://localhost/api");
                });
            }
            Assert.Equal(new[] { "first", "second" }, outgoing);
        }

        [Fact]
        public void ParameterlessRegistrationKeepsNone()
        {
            var services = new ServiceCollection();
            FunctionServices.AddFuncDistributedLoopDetector(services);
            using var provider = services.BuildServiceProvider();
            Assert.Equal(FunctionPolicy.None, provider.GetRequiredService<FunctionOptions>().AcceptedLoop);
            Assert.Throws<ArgumentOutOfRangeException>(() => FunctionServices.AddFuncDistributedLoopDetector(new ServiceCollection(), (FunctionPolicy)4));
        }

        private static ServiceProvider Services(FunctionPolicy policy, CapturedLogs logs)
        {
            var services = new ServiceCollection().AddLogging(logging => logging.AddProvider(logs));
            FunctionServices.AddFuncDistributedLoopDetector(services, policy);
            return services.BuildServiceProvider();
        }

        private static void CopyLoopId(FunctionContext source, FakeFunctionContext target)
            => target.Request.Headers.Add("X-LOOP-DETECT", (IEnumerable<string>)source.Items["X-LOOP-DETECT"]);

        private sealed class CaptureHandler : HttpMessageHandler
        {
            private readonly List<string> outgoing;
            public CaptureHandler(List<string> outgoing) { this.outgoing = outgoing; }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                outgoing.Add(Assert.Single(request.Headers.GetValues("X-LOOP-DETECT")));
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
        }
    }

    internal sealed class FakeFunctionContext : FunctionContext
    {
        public FakeFunctionContext(IServiceProvider services, string body = "a", string query = "?id=1", string header = "a", bool nonSeekable = false)
        {
            InstanceServices = services;
            Request = new FakeRequest(this, body, query, header, nonSeekable);
            var features = (FakeFeatures)Features;
            features.Set<IHttpRequestDataFeature>(new FakeRequestFeature(Request));
            features.Set<IFunctionInputBindingFeature>(new FakeInputFeature(Request));
            // The SDK's bindings feature is internal; a proxy lets its public invocation-result API
            // execute against a realistic feature without depending on gRPC worker internals.
            var bindingType = typeof(FunctionContext).Assembly.GetType("Microsoft.Azure.Functions.Worker.Context.Features.IFunctionBindingsFeature", true);
            features.Values[bindingType] = DispatchProxy.Create(bindingType, typeof(BindingsProxy));
        }
        public FakeRequest Request { get; }
        public FakeBindings Bindings { get; } = new();
        public override string InvocationId { get; } = Guid.NewGuid().ToString();
        public override string FunctionId => "function";
        public override TraceContext TraceContext => null;
        public override BindingContext BindingContext => Bindings;
        public override RetryContext RetryContext => null;
        public override IServiceProvider InstanceServices { get; set; }
        public override FunctionDefinition FunctionDefinition { get; } = new FakeDefinition();
        public override IDictionary<object, object> Items { get; set; } = new Dictionary<object, object>();
        public override IInvocationFeatures Features { get; } = new FakeFeatures();
    }

    public class BindingsProxy : DispatchProxy
    {
        private object result;
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod.Name == "get_InvocationResult") return result;
            if (targetMethod.Name == "set_InvocationResult") { result = args[0]; return null; }
            if (targetMethod.Name == "get_OutputBindingData") return new Dictionary<string, object>();
            if (targetMethod.Name == "get_TriggerMetadata" || targetMethod.Name == "get_InputData") return new Dictionary<string, object>();
            return null;
        }
    }

    internal sealed class FakeBindings : BindingContext
    {
        public Dictionary<string, object> Data { get; } = new() { ["routeId"] = "1" };
        public override IReadOnlyDictionary<string, object> BindingData => Data;
    }

    internal sealed class FakeDefinition : FunctionDefinition
    {
        public override string Name => "HttpFunction";
        public override string Id => "function";
        public override string EntryPoint => "Run";
        public override string PathToAssembly => "";
        public override ImmutableArray<FunctionParameter> Parameters => ImmutableArray<FunctionParameter>.Empty;
        public override IImmutableDictionary<string, BindingMetadata> InputBindings => ImmutableDictionary<string, BindingMetadata>.Empty;
        public override IImmutableDictionary<string, BindingMetadata> OutputBindings => ImmutableDictionary<string, BindingMetadata>.Empty;
    }

    internal sealed class FakeFeatures : IInvocationFeatures
    {
        public Dictionary<Type, object> Values { get; } = new();
        public T Get<T>() => Values.TryGetValue(typeof(T), out var value) ? (T)value : default;
        public void Set<T>(T instance) => Values[typeof(T)] = instance;
        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator() => Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal sealed class FakeRequestFeature : IHttpRequestDataFeature
    {
        private readonly HttpRequestData request;
        public FakeRequestFeature(HttpRequestData request) { this.request = request; }
        public ValueTask<HttpRequestData> GetHttpRequestDataAsync(FunctionContext context) => new(request);
    }

    internal sealed class FakeInputFeature : IFunctionInputBindingFeature
    {
        private readonly FunctionInputBindingResult result;
        public FakeInputFeature(HttpRequestData request) { result = new FunctionInputBindingResult(new object[] { request }); }
        public ValueTask<FunctionInputBindingResult> BindFunctionInputAsync(FunctionContext context) => new(result);
    }

    internal sealed class FakeRequest : HttpRequestData
    {
        public FakeRequest(FunctionContext context, string body, string query, string header, bool nonSeekable) : base(context)
        {
            Body = nonSeekable ? new NonSeekableStream(Encoding.UTF8.GetBytes(body)) : new MemoryStream(Encoding.UTF8.GetBytes(body));
            Url = new Uri("http://localhost/api" + query);
            Headers.Add("X-Value", header);
        }
        public override Stream Body { get; }
        public override HttpHeadersCollection Headers { get; } = new();
        public override IReadOnlyCollection<IHttpCookie> Cookies => Array.Empty<IHttpCookie>();
        public override Uri Url { get; }
        public override IEnumerable<ClaimsIdentity> Identities => Array.Empty<ClaimsIdentity>();
        public override string Method => "POST";
        public override HttpResponseData CreateResponse() => new FakeResponse(FunctionContext);
    }

    internal sealed class FakeResponse : HttpResponseData
    {
        public FakeResponse(FunctionContext context) : base(context) { }
        public override HttpStatusCode StatusCode { get; set; }
        public override HttpHeadersCollection Headers { get; set; } = new();
        public override Stream Body { get; set; } = new MemoryStream();
        public override HttpCookies Cookies => throw new NotSupportedException();
    }

    internal sealed class NonSeekableStream : MemoryStream
    {
        public NonSeekableStream(byte[] bytes) : base(bytes) { }
        public override bool CanSeek => false;
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    }
}
