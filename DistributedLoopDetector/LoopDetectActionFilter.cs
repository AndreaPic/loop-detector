﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace DistributedLoopDetector
{
    /// <summary>
    /// This filter add current loop context during for this specific Action Execution
    /// </summary>
    public class LoopDetectActionFilter : IActionFilter
    {
        /// <summary>
        /// current loopId (identify current loop context)
        /// </summary>
        private string? loopId = null;

        /// <summary>
        /// When an action is starting look for a loopid in message header.
        /// If loopid is present uses it as current loopid context otherwise creates a new loopid
        /// </summary>
        /// <param name="context">
        /// Current execution context
        /// </param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string? path = context?.HttpContext?.Request?.Path;
            if ( (path != null) && (context!=null))
            {
                if (!context.HttpContext.Request.Headers.ContainsKey(LoopDetectorHandler.HeaderName))
                {
                    loopId = Guid.NewGuid().ToString("N");
                    LoopDetectStack.Instance.AddLoopDetectInfo(path, loopId);
                    context.HttpContext.Items.Add(LoopDetectorHandler.HeaderName, loopId);
                    context.HttpContext.Request.Headers.Add(LoopDetectorHandler.HeaderName, loopId);

                    System.Diagnostics.Debug.WriteLine($"WAITHING FOR NEW: {loopId}");

                }
                else
                {
                    int count = 0;
                    bool acquired = false;
                    StringValues objLoopId;
                    do
                    {
                        acquired = context.HttpContext.Request.Headers.TryGetValue(LoopDetectorHandler.HeaderName, out objLoopId);
                        count++;
                    } 
                    while (!acquired && count < 3);
                    LoopDetectStack.Instance.AddLoopDetectInfo(path, objLoopId);
                    loopId = objLoopId;
                    context.HttpContext.Items.Add(LoopDetectorHandler.HeaderName, objLoopId);
                    System.Diagnostics.Debug.WriteLine($"WAITHING FOR READED: {loopId}");
                }
            }
        }

        /// <summary>
        /// When action ended remove current loop context
        /// </summary>
        /// <param name="context">
        /// Action executed context
        /// </param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context is not null)
            {
                if (context.Exception is TimeoutException || context.Exception is TaskCanceledException)
                {
                    return;
                }
                
                if (context?.HttpContext?.Items != null)
                {
                    if (!context.HttpContext.Items.ContainsKey(HttpStatusCode.RequestTimeout.ToString()))
                    {
                        string path = context.HttpContext?.Request?.Path!;
                        if (path != null && loopId != null)
                        {
                            LoopDetectStack.Instance.RemoveLoopDetectInfo(path, loopId);
                            if (context.HttpContext != null)
                            {
                                context.HttpContext.Items.Remove(LoopDetectorHandler.HeaderName);
                            }
                            loopId = null;
                        }
                    }
                }
            }
        }



    }
}
