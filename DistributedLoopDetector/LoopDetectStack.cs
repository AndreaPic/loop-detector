using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace DistributedLoopDetector
{
    /// <summary>
    /// Stack that record every concurrent loop context
    /// </summary>
    public sealed class LoopDetectStack
    {
        /// <summary>
        /// Multidimensional repository that memorize currents loop context
        /// </summary>
        private ConcurrentDictionary<string, HashSet<string>> LoopDetectActions;

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static LoopDetectStack _instance;

        /// <summary>
        /// critical section handler
        /// </summary>        
        private static volatile object SyncLock = new();

        /// <summary>
        /// Private default constructor
        /// </summary>
        private LoopDetectStack()
        {
            LoopDetectActions = new ConcurrentDictionary<string, HashSet<string>>();
            synkObj = new object();
        }
        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static LoopDetectStack Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                lock (SyncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new LoopDetectStack();
                    }
                }
                return _instance;
            }
        }

        private IDistributedCache DistributedCache { get; set; }
        private string ApplicationName { get; set; }
        public void SetDistributedCache(IDistributedCache distributedCache,string applicationName)
        {
            DistributedCache = distributedCache;
            ApplicationName = applicationName;
        }
        private IDistributedCache GetDistributedCache()
        {
            return DistributedCache;
        }
        private bool UseDistributedCache
        {
            get
            {
                return DistributedCache!=null;
            }
        }



        /// <summary>
        /// inner dimension critical section handler
        /// </summary>
        private volatile object synkObj;

        /// <summary>
        /// Compose key value for DistributedCache
        /// </summary>
        /// <param name="actionName">api action name</param>
        /// <param name="loopDetectId">loop context id</param>
        /// <returns></returns>
        private string ComposeKey(string actionName, string loopDetectId)
        {
            return $"{ApplicationName}-{actionName}-{loopDetectId}";
        }

        /// <summary>
        /// Check if there is a loopid for a specific action
        /// </summary>
        /// <param name="actionName">Action to look for</param>
        /// <param name="loopDetectId">LoopId to look for</param>
        /// <returns>True if there is the loop id for the requested action</returns>
        internal bool LoopDetectInfoMatch(string actionName, string loopDetectId)
        {
            if (UseDistributedCache)
            {
                lock (synkObj)
                {
                    var cacheValue = DistributedCache.GetString(ComposeKey(actionName, loopDetectId));
                    if (cacheValue != null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                var actionPresent = LoopDetectActions.TryGetValue(actionName, out var list);
                if (actionPresent && list is not null)
                {
                    lock (synkObj)
                    {
                        if (list.Contains(loopDetectId))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /*
        /// <summary>
        /// Get all active loopid for a specified Action
        /// </summary>
        /// <param name="actionName">Action to look for</param>
        /// <returns>active loop id list</returns>
        internal IReadOnlyList<string> GetDetectInfo(string actionName)
        {
            var actionPresent = LoopDetectActions.TryGetValue(actionName, out var list);
            if (actionPresent)
            {
                if (list != null)
                {
                    lock (synkObj)
                    {
                        return list.ToList();
                    }
                }
                else
                {
                    return new List<string>();
                }
            }
            else
            {
                return new List<string>();
            }
        }
        */

        /// <summary>
        /// Add a new loopid for an action
        /// </summary>
        /// <param name="actionName">Action for the new loopid</param>
        /// <param name="loopDetectId">A new loop for the action</param>
        internal void AddLoopDetectInfo(string actionName, string loopDetectId)
        {
            if (UseDistributedCache)
            {
                lock (synkObj)
                {
                    DistributedCache.SetString(ComposeKey(actionName, loopDetectId), loopDetectId);
                }
            }
            else
            {

                LoopDetectActions.AddOrUpdate(actionName,
                                        new HashSet<string>(new[] { loopDetectId }),
                                        (key, bag) =>
                                        {
                                            lock (synkObj)
                                            {
                                                if (!bag.Contains(loopDetectId))
                                                {
                                                    bag.Add(loopDetectId);
                                                }
                                            }
                                            return bag;
                                        });
            }
        }

        /// <summary>
        /// Remove a loopId for specified action
        /// </summary>
        /// <param name="actionName">Action where remove the loopid</param>
        /// <param name="loopDetectId">loopId to remove</param>
        /// <returns>True if removed</returns>
        internal bool RemoveLoopDetectInfo(string actionName, string loopDetectId)
        {
            bool removed = false;
            bool fullRemoved = false;

            if (UseDistributedCache)
            {
                lock (synkObj)
                {
                    DistributedCache.Remove(ComposeKey(actionName, loopDetectId));
                    return true;
                }
            }
            else
            {
                var founded = LoopDetectActions.TryGetValue(actionName, out var list);
                lock (synkObj)
                {
                    if (founded && list != null)
                    {
                        removed = list.Remove(loopDetectId);
                        if (list.Count == 0)
                        {
                            fullRemoved = LoopDetectActions.Remove(actionName, out var removedItem);
                        }
                    }
                    return (removed || fullRemoved);
                }
            }
        }
    }
}
