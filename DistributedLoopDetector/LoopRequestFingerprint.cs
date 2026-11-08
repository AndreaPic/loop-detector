using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;

namespace DistributedLoopDetector
{
    // Only hashes are retained in active-request storage. No request content is propagated.
    internal sealed record LoopRequestFingerprint(string? Payload, string? Headers)
    {
        private static readonly HashSet<string> IgnoredHeaders = new(StringComparer.OrdinalIgnoreCase)
        {
            LoopDetectorHandler.HeaderName, "traceparent", "tracestate", "Request-Id", "baggage", "Correlation-Context"
        };

        internal static async Task<LoopRequestFingerprint> CreateAsync(HttpRequest request, AcceptedLoop acceptedLoop,
            CancellationToken cancellationToken = default)
        {
            string? payload = null;
            string? headers = null;
            if (acceptedLoop.HasFlag(AcceptedLoop.WithDifferentPayload))
            {
                request.EnableBuffering();
                long position = request.Body.Position;
                string body;
                try
                {
                    request.Body.Position = 0;
                    using var sha = SHA256.Create();
                    body = Convert.ToHexString(await sha.ComputeHashAsync(request.Body, cancellationToken));
                }
                finally
                {
                    request.Body.Position = position;
                }

                payload = Hash(new
                {
                    Path = request.Path.Value,
                    Body = body,
                    Query = request.Query.OrderBy(pair => pair.Key, StringComparer.Ordinal)
                        .Select(pair => new { pair.Key, Values = pair.Value.ToArray() }),
                    Route = request.RouteValues.OrderBy(pair => pair.Key, StringComparer.Ordinal)
                        .Select(pair => new { pair.Key, Value = Convert.ToString(pair.Value, CultureInfo.InvariantCulture) })
                });
            }
            if (acceptedLoop.HasFlag(AcceptedLoop.WithDifferentHeaders))
            {
                headers = Hash(request.Headers.Where(pair => !IgnoredHeaders.Contains(pair.Key))
                    .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                    .Select(pair => new { Key = pair.Key.ToUpperInvariant(), Values = pair.Value.ToArray() }));
            }
            return new LoopRequestFingerprint(payload, headers);
        }

        internal bool Matches(LoopRequestFingerprint other, AcceptedLoop acceptedLoop)
        {
            // An old registration without the requested fingerprint remains a blocking loop.
            bool samePayload = !acceptedLoop.HasFlag(AcceptedLoop.WithDifferentPayload)
                || Payload == null || other.Payload == null || Payload == other.Payload;
            bool sameHeaders = !acceptedLoop.HasFlag(AcceptedLoop.WithDifferentHeaders)
                || Headers == null || other.Headers == null || Headers == other.Headers;
            return samePayload && sameHeaders;
        }

        private static string Hash<T>(T value) => Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(value)));
    }
}
