using DistributedLoopDetector;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DistributedLoopDetectorTests
{
    public class AcceptedLoopTests
    {
        [Theory]
        [InlineData(AcceptedLoop.None, false)]
        [InlineData(AcceptedLoop.WithDifferentPayload, true)]
        [InlineData(AcceptedLoop.WithDifferentHeaders, false)]
        [InlineData(AcceptedLoop.WithDifferentPayload | AcceptedLoop.WithDifferentHeaders, true)]
        public void OnlySelectedDifferencesAllowRequests(AcceptedLoop policy, bool allowed)
        {
            var stack = new LoopDetectStackInstance();
            var ids = new[] { "loop" };
            Assert.NotNull(stack.TryRegisterRequest("api", ids, new("payload-a", "headers-a"), policy, out var firstExcluded));
            Assert.False(firstExcluded);
            var registration = stack.TryRegisterRequest("api", ids, new("payload-b", "headers-a"), policy, out var excluded);
            Assert.Equal(allowed, registration != null);
            Assert.True(excluded);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EachVariationHasItsOwnLifetime(bool distributed)
        {
            using var provider = new ServiceCollection().AddDistributedMemoryCache().BuildServiceProvider();
            var stack = new LoopDetectStackInstance();
            var otherInstance = distributed ? new LoopDetectStackInstance() : stack;
            if (distributed)
            {
                var cache = provider.GetRequiredService<IDistributedCache>();
                stack.SetDistributedCache(cache, "app");
                otherInstance.SetDistributedCache(cache, "app");
            }
            var ids = new[] { "loop" };
            var a = new LoopRequestFingerprint("a", null);
            var b = new LoopRequestFingerprint("b", null);
            var policy = AcceptedLoop.WithDifferentPayload;
            var first = stack.TryRegisterRequest("api", ids, a, policy, out _);
            var second = otherInstance.TryRegisterRequest("api", ids, b, policy, out var excluded);
            Assert.NotNull(second);
            Assert.True(excluded);
            Assert.Null(otherInstance.TryRegisterRequest("api", ids, a, policy, out _));
            Assert.Null(stack.TryRegisterRequest("api", ids, b, policy, out _));
            otherInstance.RemoveRequest("api", ids, second);
            Assert.True(stack.LoopDetectInfoMatch("api", "loop"));
            Assert.Null(otherInstance.TryRegisterRequest("api", ids, a, policy, out _));
            var again = otherInstance.TryRegisterRequest("api", ids, b, policy, out _);
            Assert.NotNull(again);
            stack.RemoveRequest("api", ids, first);
            Assert.True(stack.LoopDetectInfoMatch("api", "loop"));
            otherInstance.RemoveRequest("api", ids, again);
            Assert.False(stack.LoopDetectInfoMatch("api", "loop"));
        }

        [Fact]
        public async Task ConcurrentIdenticalRequestsOnlyRegisterOnce()
        {
            var stack = new LoopDetectStackInstance();
            var results = await Task.WhenAll(Enumerable.Range(0, 50).Select(index => Task.Run(() =>
                stack.TryRegisterRequest("api", new[] { "loop" }, new("a", "b"), AcceptedLoop.WithDifferentPayload, out _))));
            Assert.Single(results, result => result != null);
        }

        [Fact]
        public void AnyMatchingLoopIdBlocksWithoutRegisteringOtherIds()
        {
            var stack = new LoopDetectStackInstance();
            stack.TryRegisterRequest("api", new[] { "existing" }, new("a", null), AcceptedLoop.WithDifferentPayload, out _);
            Assert.Null(stack.TryRegisterRequest("api", new[] { "new", "existing" }, new("a", null), AcceptedLoop.WithDifferentPayload, out _));
            Assert.False(stack.LoopDetectInfoMatch("api", "new"));
        }

        [Fact]
        public void OldCacheEntriesRemainBlocking()
        {
            using var provider = new ServiceCollection().AddDistributedMemoryCache().BuildServiceProvider();
            var cache = provider.GetRequiredService<IDistributedCache>();
            cache.SetString("app-api-loop", "loop");
            var stack = new LoopDetectStackInstance();
            stack.SetDistributedCache(cache, "app");
            Assert.Null(stack.TryRegisterRequest("api", new[] { "loop" }, new("new", "new"), AcceptedLoop.WithDifferentPayload, out _));
        }

        [Fact]
        public async Task FingerprintsNormalizeNamesAndPreserveBodiesAndRepeatedValues()
        {
            var first = Request("body", "?a=1&b=2&a=3");
            first.Headers["X-Custom"] = "value";
            first.Headers["X-LOOP-DETECT"] = "first";
            first.Headers["traceparent"] = "trace-one";
            var second = Request("body", "?b=2&a=1&a=3");
            second.Headers["x-custom"] = "value";
            second.Headers["x-loop-detect"] = "second";
            second.Headers["traceparent"] = "trace-two";
            var policy = AcceptedLoop.WithDifferentPayload | AcceptedLoop.WithDifferentHeaders;
            var fingerprint = await LoopRequestFingerprint.CreateAsync(first, policy);
            Assert.Equal(fingerprint, await LoopRequestFingerprint.CreateAsync(second, policy));
            Assert.Equal(0, first.Body.Position);
            Assert.Equal("body", await new StreamReader(first.Body).ReadToEndAsync());
            second.QueryString = new QueryString("?b=2&a=3&a=1");
            Assert.NotEqual(fingerprint.Payload, (await LoopRequestFingerprint.CreateAsync(second, policy)).Payload);
            second.RouteValues["id"] = 5;
            var routeFingerprint = await LoopRequestFingerprint.CreateAsync(second, policy);
            second.RouteValues["id"] = 6;
            Assert.NotEqual(routeFingerprint.Payload, (await LoopRequestFingerprint.CreateAsync(second, policy)).Payload);
        }

        [Fact]
        public async Task NoneAndHeadersDoNotReadTheBody()
        {
            var request = new DefaultHttpContext().Request;
            request.Body = new UnreadableStream();
            Assert.Equal(new LoopRequestFingerprint(null, null), await LoopRequestFingerprint.CreateAsync(request, AcceptedLoop.None));
            Assert.NotNull((await LoopRequestFingerprint.CreateAsync(request, AcceptedLoop.WithDifferentHeaders)).Headers);
        }

        [Fact]
        public async Task DistributedStateContainsFingerprintsWithoutRequestContent()
        {
            using var provider = new ServiceCollection().AddDistributedMemoryCache().BuildServiceProvider();
            var cache = provider.GetRequiredService<IDistributedCache>();
            var stack = new LoopDetectStackInstance();
            stack.SetDistributedCache(cache, "app");
            var request = Request("private-body", "?secret=private-query");
            request.Headers["Authorization"] = "Bearer private-token";
            var policy = AcceptedLoop.WithDifferentPayload | AcceptedLoop.WithDifferentHeaders;
            var fingerprint = await LoopRequestFingerprint.CreateAsync(request, policy);
            stack.TryRegisterRequest("api", new[] { "loop" }, fingerprint, policy, out _);
            string state = cache.GetString("app-api-loop");
            Assert.DoesNotContain("private-body", state);
            Assert.DoesNotContain("private-query", state);
            Assert.DoesNotContain("private-token", state);
            Assert.Contains(fingerprint.Payload, state);
            Assert.Contains(fingerprint.Headers, state);
        }

        [Fact]
        public void RegistrationDefaultsToNoneAndRejectsUnknownFlags()
        {
            using var provider = new ServiceCollection().AddDistributedLoopDetector().BuildServiceProvider();
            Assert.Equal(AcceptedLoop.None, provider.GetRequiredService<LoopDetectorOptions>().AcceptedLoop);
            Assert.Throws<ArgumentOutOfRangeException>(() => new ServiceCollection().AddDistributedLoopDetector((AcceptedLoop)4));
        }

        private static HttpRequest Request(string body, string query)
        {
            var request = new DefaultHttpContext().Request;
            request.Path = "/api";
            request.QueryString = new QueryString(query);
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            return request;
        }

        private sealed class UnreadableStream : Stream
        {
            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override int Read(byte[] buffer, int offset, int count) => throw new InvalidOperationException("Body must not be read.");
            public override void Flush() => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }
    }
}
