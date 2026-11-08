using DistributedLoopDetector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace DistributedLoopDetectorTests
{
    public class AspNetAcceptedLoopTests
    {
        [Theory]
        [InlineData(AcceptedLoop.None, "body", 508)]
        [InlineData(AcceptedLoop.None, "query", 508)]
        [InlineData(AcceptedLoop.None, "headers", 508)]
        [InlineData(AcceptedLoop.WithDifferentPayload, "body", 200)]
        [InlineData(AcceptedLoop.WithDifferentPayload, "query", 200)]
        [InlineData(AcceptedLoop.WithDifferentPayload, "headers", 508)]
        [InlineData(AcceptedLoop.WithDifferentHeaders, "body", 508)]
        [InlineData(AcceptedLoop.WithDifferentHeaders, "headers", 200)]
        [InlineData(AcceptedLoop.WithDifferentPayload | AcceptedLoop.WithDifferentHeaders, "body", 200)]
        [InlineData(AcceptedLoop.WithDifferentPayload | AcceptedLoop.WithDifferentHeaders, "headers", 200)]
        public async Task OptionsControlRealMvcRequests(AcceptedLoop policy, string difference, int expectedStatus)
        {
            var first = new RequestSpec();
            var second = difference switch
            {
                "body" => first with { Value = "b" },
                "query" => first with { Query = "?id=2" },
                _ => first with { Header = "b" }
            };
            var state = new RequestSequence(first, second);
            using var server = Server(policy, state);
            using var client = server.GetTestClient();
            using var request = first.Create();
            using var response = await client.SendAsync(request);
            Assert.Equal(expectedStatus, (int)response.StatusCode);
            Assert.Equal(expectedStatus == 200 ? 2 : 1, state.Values.Count);
            Assert.Equal("a", state.Values[0]);
            if (expectedStatus == 200)
            {
                Assert.Equal(second.Value, state.Values[1]);
                Assert.Contains(state.Logs.Messages, message => message.Level == LogLevel.Warning
                    && message.Message.Contains("excluded by") && message.Message.Contains(policy.ToString()));
            }
        }

        [Theory]
        [InlineData(AcceptedLoop.WithDifferentPayload)]
        [InlineData(AcceptedLoop.WithDifferentHeaders)]
        [InlineData(AcceptedLoop.WithDifferentPayload | AcceptedLoop.WithDifferentHeaders)]
        public async Task ReturningToEarlierVariationIsBlocked(AcceptedLoop policy)
        {
            var first = new RequestSpec();
            var state = new RequestSequence(first, first with { Value = "b", Header = "b" }, first);
            using var server = Server(policy, state);
            using var client = server.GetTestClient();
            using var request = first.Create();
            using var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.LoopDetected, response.StatusCode);
            Assert.Equal(2, state.Values.Count);
        }

        [Theory]
        [InlineData(AcceptedLoop.None)]
        [InlineData(AcceptedLoop.WithDifferentPayload)]
        [InlineData(AcceptedLoop.WithDifferentHeaders)]
        public async Task IdenticalRequestsAndTracingOnlyDifferencesAreBlocked(AcceptedLoop policy)
        {
            var first = new RequestSpec();
            var state = new RequestSequence(first, first with { Trace = "00-22222222222222222222222222222222-2222222222222222-01" });
            using var server = Server(policy, state);
            using var client = server.GetTestClient();
            using var request = first.Create();
            using var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.LoopDetected, response.StatusCode);
            Assert.Single(state.Values);
        }

        private static IHost Server(AcceptedLoop policy, RequestSequence sequence)
        {
            IHost server = null;
            server = new HostBuilder().ConfigureWebHost(web => web.UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddLogging(logging => logging.AddProvider(sequence.Logs));
                    services.AddDistributedLoopDetector(policy);
                    services.AddControllers().AddApplicationPart(typeof(LoopTestController).Assembly);
                    services.AddSingleton(sequence);
                    services.AddHttpClient("loop").ConfigurePrimaryHttpMessageHandler(() => server.GetTestServer().CreateHandler());
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                })).Start();
            return server;
        }
    }

    public record RequestSpec(string Value = "a", string Query = "?id=1", string Header = "a",
        string Trace = "00-11111111111111111111111111111111-1111111111111111-01")
    {
        public HttpRequestMessage Create()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/loop" + Query)
            {
                Content = JsonContent.Create(new LoopPayload { Value = Value })
            };
            request.Headers.Add("X-Value", Header);
            request.Headers.Add("traceparent", Trace);
            return request;
        }
    }

    public class LoopPayload { public string Value { get; set; } }

    public sealed class RequestSequence
    {
        public RequestSequence(params RequestSpec[] requests) { Requests = requests; }
        public RequestSpec[] Requests { get; }
        public List<string> Values { get; } = new();
        public CapturedLogs Logs { get; } = new();
    }

    [ApiController]
    public class LoopTestController : ControllerBase
    {
        private readonly RequestSequence sequence;
        private readonly IHttpClientFactory clients;
        public LoopTestController(RequestSequence sequence, IHttpClientFactory clients)
        {
            this.sequence = sequence;
            this.clients = clients;
        }

        [HttpPost("/loop")]
        public async Task<IActionResult> Post([FromBody] LoopPayload payload)
        {
            sequence.Values.Add(payload.Value);
            int index = sequence.Values.Count;
            if (index == sequence.Requests.Length)
                return Ok();
            using var request = sequence.Requests[index].Create();
            using var client = clients.CreateClient("loop");
            client.BaseAddress = new Uri("http://localhost");
            using var response = await client.SendAsync(request);
            return StatusCode((int)response.StatusCode);
        }
    }

    public sealed class CapturedLogs : ILoggerProvider, ILogger
    {
        public ConcurrentQueue<(LogLevel Level, string Message)> Messages { get; } = new();
        public ILogger CreateLogger(string categoryName) => this;
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => Messages.Enqueue((logLevel, formatter(state, exception)));
        public void Dispose() { }
    }
}
