using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DistributedLoopDetector
{
    /// <summary>
    /// This resourcefilter return http error if Loop is detected
    /// </summary>
    public class LoopDetectResourceFilter : IResourceFilter
    {
        /// <summary>
        /// Do nothing
        /// </summary>
        /// <param name="context">Executed context</param>
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
        /// <summary>
        /// If there is a current loop id context inside the request's header its return http error
        /// </summary>
        /// <param name="context">Executing context</param>
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context is not null)
            {
                var headers = context!.HttpContext?.Request?.Headers;
                string? path = context.HttpContext?.Request?.Path;
                if ((headers != null) && (!string.IsNullOrEmpty(path)))
                {
                    var header = headers.FirstOrDefault(head => head.Key == LoopDetectorHandler.HeaderName);
                    bool loopDetected = header.Value.FirstOrDefault(item => LoopDetectStack.Instance.LoopDetectInfoMatch(path, item)) != null;
                    if (loopDetected)
                    {
                        context.Result = new StatusCodeResult(508);
                    }
                }
            }            
        }
    }
}
