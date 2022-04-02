using DistributedLoopDetector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace DistributedLoopDetectorTests
{
    public class LoopDetectStackTest
    {

        [Fact]
        public void AddTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                LoopDetectStack.Instance.AddLoopDetectInfo("actionName", key);
                LoopDetectStack.Instance.AddLoopDetectInfo("actionName", key);
                var match = LoopDetectStack.Instance.LoopDetectInfoMatch("actionName", key);
                Assert.True(match);
                match = LoopDetectStack.Instance.LoopDetectInfoMatch("actionName", Guid.NewGuid().ToString("N"));
                Assert.False(match);
            }
        }

        [Fact]
        public void RemoveTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                LoopDetectStack.Instance.AddLoopDetectInfo("actionName", key);
                LoopDetectStack.Instance.AddLoopDetectInfo("actionName", key);
                var removed = LoopDetectStack.Instance.RemoveLoopDetectInfo("actionName", key);
                Assert.True(removed);
                removed = LoopDetectStack.Instance.RemoveLoopDetectInfo("actionName", key);
                Assert.False(removed);
            }
        }


        private void Check(object? state)
        {
            string key = (string)state;
            LoopDetectStack.Instance.AddLoopDetectInfo("actionName", key);
            var match = LoopDetectStack.Instance.LoopDetectInfoMatch("actionName", key);
            Assert.True(match);
            var removed = LoopDetectStack.Instance.RemoveLoopDetectInfo("actionName", key);
            Assert.True(removed);
            match = LoopDetectStack.Instance.LoopDetectInfoMatch("actionName", key);
            Assert.False(match);
        }

        private void Add(object? state)
        {
            string key = (string)state;
            Debug.WriteLine(key);
            LoopDetectStack.Instance.AddLoopDetectInfo("actionName", key);
        }

        private volatile object sync = new object();

        int counter = 0;
        private void Exists(Task t, object? state)
        {
            string key = (string)state;
            var match = LoopDetectStack.Instance.LoopDetectInfoMatch("actionName", key);
            Assert.True(match);
        }
        private void NotExists(Task t, object? state)
        {
            //Interlocked.Increment(ref counter);
            string key = (string)state;
            var match = LoopDetectStack.Instance.LoopDetectInfoMatch("actionName", key);
            Assert.False(match);
        }
        private void Remove(Task t, object? state)
        {
            string key = (string)state;
            var removed = LoopDetectStack.Instance.RemoveLoopDetectInfo("actionName", key);
            Assert.True(removed);
        }

        ConcurrentBag<string> mainLoops = new ConcurrentBag<string>();

        [Theory]
        [InlineData(short.MaxValue)]
        public void ThreadTest(int threadCount)
        {
            for(int i = 0; i < threadCount; i++)
            {
                mainLoops.Add(Guid.NewGuid().ToString("N"));
            }
            List<Task> tasks = new List<Task>();
            bool passed = true;
            int count = mainLoops.Count;

            for (int l = 0; l < count-1; l++)
            {
                mainLoops.TryTake(out string key);
                //Debug.WriteLine(key);
                //tasks.Add(Task.Factory.StartNew(() => Check(key))
                //.ContinueWith(t =>
                //{
                //    if (t.Exception != null)
                //    {
                //        passed = false;
                //    }
                //    Assert.Null(t.Exception);
                //}));

                tasks.Add(Task.Factory.StartNew(() => Add(key))
                .ContinueWith(t => Exists(t, key))
                .ContinueWith(t => Remove(t, key))
                .ContinueWith(t => NotExists(t, key))
                .ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        passed = false;
                    }
                    Assert.Null(t.Exception);
                }));
            }
            
            Task.WaitAll(tasks.ToArray());
            Debug.WriteLine(counter.ToString());
            Assert.True(passed);

        }
    }
}