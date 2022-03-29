using System.Collections.Concurrent;

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
        private ConcurrentDictionary<string, List<string>> LoopDetectActions 
            = new ConcurrentDictionary<string, List<string>>();

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static volatile LoopDetectStack _instance;
        /// <summary>
        /// critical section handler
        /// </summary>
        private static readonly object SyncLock = new();
        /// <summary>
        /// Private default constructor
        /// </summary>
        private LoopDetectStack()
        {
        }
        /// <summary>
        /// Singleton Intance
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

        /// <summary>
        /// inner dimension critical section handler
        /// </summary>
        private readonly object synkObj = new object();

        /// <summary>
        /// Check if there is a loopid for a specific action
        /// </summary>
        /// <param name="actionName">Action to look for</param>
        /// <param name="loopDetectId">LoopId to look for</param>
        /// <returns>True if there is the loop id for the requested action</returns>
        internal bool LoopDetectInfoMatch(string actionName, string loopDetectId)
        {
            var actionPresent = LoopDetectActions.TryGetValue(actionName, out var list);
            if (actionPresent)
            {
                lock (synkObj)
                {
                    if (list.Contains(loopDetectId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
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
                return list.ToList();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Add a new loopid for an action
        /// </summary>
        /// <param name="actionName">Action for the new loopid</param>
        /// <param name="loopDetectId">A new loop for the action</param>
        internal void AddLoopDetectInfo(string actionName, string loopDetectId)
        {
            LoopDetectActions.AddOrUpdate(actionName, new List<string>(new [] {loopDetectId}), (key,bag) =>
                                        {
                                            lock (synkObj)
                                            {
                                                bag.Add(loopDetectId);
                                            }
                                            return bag;
                                        });
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
            lock (synkObj)
            { 
                var founded = LoopDetectActions.TryGetValue(actionName, out var list);
                if (founded && list != null)
                {
                    removed = list.Remove(loopDetectId);
                    if (list.Count == 0)
                    {
                        fullRemoved = LoopDetectActions.Remove(actionName, out var removedItem);
                    }
                }
            }
            return removed || fullRemoved;
        }
    }
}
