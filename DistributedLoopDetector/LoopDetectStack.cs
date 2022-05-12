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
        /// Singleton instance
        /// </summary>
        private static LoopDetectStackInstance _instance;

        /// <summary>
        /// critical section handler
        /// </summary>        
        private static volatile object SyncLock = new();

        /// <summary>
        /// Private default constructor
        /// </summary>
        private LoopDetectStack()
        {
        }
        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static LoopDetectStackInstance Instance
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
                        _instance = new LoopDetectStackInstance();
                    }
                }
                return _instance;
            }
        }

    }
}
