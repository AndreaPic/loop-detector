using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
        private string loopId = null;

        /// <summary>
        /// When an action is starting look for a loopid in message header.
        /// If loopid is present uses it as current loopid context otherwise creates a new loopid
        /// </summary>
        /// <param name="context">
        /// Current execution context
        /// </param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string path = context?.HttpContext?.Request?.Path;
            if ( (path != null) && (context!=null))
            {
                if (!context.HttpContext.Request.Headers.ContainsKey(LoopDetectorHandler.HeaderName))
                {
                    loopId = Guid.NewGuid().ToString("N");
                    LoopDetectStack.Instance.AddLoopDetectInfo(path, loopId);
                    context.HttpContext.Items.Add(LoopDetectorHandler.HeaderName, loopId);
                    context.HttpContext.Request.Headers.Add(LoopDetectorHandler.HeaderName, loopId);
                }
                else
                {
                    context.HttpContext.Request.Headers.TryGetValue(LoopDetectorHandler.HeaderName, out var objLoopId);
                    LoopDetectStack.Instance.AddLoopDetectInfo(path, objLoopId);
                    loopId = objLoopId;
                    context.HttpContext.Items.Add(LoopDetectorHandler.HeaderName, objLoopId);
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
            string path = context?.HttpContext?.Request?.Path;
            if (path != null && loopId != null)
            {
                LoopDetectStack.Instance.RemoveLoopDetectInfo(path, loopId);
                context.HttpContext.Items.Remove(LoopDetectorHandler.HeaderName);
                loopId = null;
            }
        }



    }
}
