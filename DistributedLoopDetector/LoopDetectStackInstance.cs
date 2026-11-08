using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DistributedLoopDetector
{
    /// <summary>Tracks active API requests in local memory or the configured distributed cache.</summary>
    public sealed class LoopDetectStackInstance
    {
        private const string LegacyRegistration = "legacy";
        private const string CacheFormat = "requests-v1:";
        // Serialize updates between stack instances using the same in-process cache provider.
        private static readonly object SyncLock = new();
        private readonly Dictionary<(string Action, string LoopId), Dictionary<string, LoopRequestFingerprint?>> requests = new();
        private IDistributedCache? distributedCache;
        private string? applicationName;

        internal void SetDistributedCache(IDistributedCache cache, string name)
        {
            distributedCache = cache;
            applicationName = name;
        }

        private string ComposeKey(string actionName, string loopDetectId) => $"{applicationName}-{actionName}-{loopDetectId}";

        private Dictionary<string, LoopRequestFingerprint?> Read(string actionName, string loopDetectId)
        {
            if (distributedCache == null)
                return requests.TryGetValue((actionName, loopDetectId), out var active)
                    ? active : new Dictionary<string, LoopRequestFingerprint?>();

            var value = distributedCache.GetString(ComposeKey(actionName, loopDetectId));
            if (value == null)
                return new Dictionary<string, LoopRequestFingerprint?>();
            if (!value.StartsWith(CacheFormat, StringComparison.Ordinal))
            {
                // Entries from previous library versions contain only the loop ID.
                return new Dictionary<string, LoopRequestFingerprint?> { [LegacyRegistration] = null };
            }
            return JsonSerializer.Deserialize<Dictionary<string, LoopRequestFingerprint?>>(value.Substring(CacheFormat.Length))!;
        }

        private void Write(string actionName, string loopDetectId, Dictionary<string, LoopRequestFingerprint?> active)
        {
            if (distributedCache != null)
            {
                var key = ComposeKey(actionName, loopDetectId);
                if (active.Count == 0)
                    distributedCache.Remove(key);
                else
                    distributedCache.SetString(key, CacheFormat + JsonSerializer.Serialize(active));
            }
            else if (active.Count == 0)
                requests.Remove((actionName, loopDetectId));
            else
                requests[(actionName, loopDetectId)] = active;
        }

        internal bool LoopDetectInfoMatch(string actionName, string loopDetectId)
        {
            lock (SyncLock)
                return Read(actionName, loopDetectId).Count != 0;
        }

        internal void AddLoopDetectInfo(string actionName, string loopDetectId)
        {
            lock (SyncLock)
            {
                var active = Read(actionName, loopDetectId);
                active[LegacyRegistration] = null;
                Write(actionName, loopDetectId, active);
            }
        }

        internal bool RemoveLoopDetectInfo(string actionName, string loopDetectId)
        {
            lock (SyncLock)
            {
                var active = Read(actionName, loopDetectId);
                bool removed = active.Remove(LegacyRegistration);
                Write(actionName, loopDetectId, active);
                return distributedCache != null || removed;
            }
        }

        // Check and registration share a critical section to prevent local check/add races.
        internal string? TryRegisterRequest(string actionName, IReadOnlyCollection<string> loopIds,
            LoopRequestFingerprint fingerprint, AcceptedLoop acceptedLoop, out bool excludedLoop)
        {
            lock (SyncLock)
            {
                var states = loopIds.Distinct(StringComparer.Ordinal).ToDictionary(id => id, id => Read(actionName, id));
                excludedLoop = states.Values.Any(active => active.Count != 0);
                if (states.Values.Any(active => active.Values.Any(previous => previous == null || previous.Matches(fingerprint, acceptedLoop))))
                    return null;

                var registration = Guid.NewGuid().ToString("N");
                foreach (var state in states)
                {
                    state.Value[registration] = fingerprint;
                    Write(actionName, state.Key, state.Value);
                }
                return registration;
            }
        }

        internal void RemoveRequest(string actionName, IEnumerable<string> loopIds, string registration)
        {
            lock (SyncLock)
            {
                foreach (var id in loopIds.Distinct(StringComparer.Ordinal))
                {
                    var active = Read(actionName, id);
                    active.Remove(registration);
                    Write(actionName, id, active);
                }
            }
        }
    }
}
