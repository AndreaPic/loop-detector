using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace DistributedLoopDetector
{
    /// <summary>
    /// Add the current loop id context to the header for the http calls
    /// </summary>
    public class LoopDetectorHandler : DelegatingHandler
    {
        /// <summary>
        /// Current http context accessor
        /// </summary>
        private IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Constructor for the DI
        /// </summary>
        /// <param name="contextAccessor">Required context accessor</param>
        public LoopDetectorHandler(IHttpContextAccessor contextAccessor)
        {
            httpContextAccessor = contextAccessor;
        }
        /// <summary>
        /// Header Name
        /// </summary>
        internal const string HeaderName = "X-LOOP-DETECT";
        /// <summary>
        /// Propagate the loop id context through the http header
        /// </summary>
        /// <param name="request">Original http request</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Request's response</returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            object objHeaderLoopIds;
            var idExists = httpContextAccessor.HttpContext.Items.TryGetValue(LoopDetectorHandler.HeaderName, out objHeaderLoopIds);
            if (idExists)
            {
                string headerValue;
                headerValue = objHeaderLoopIds as string;
                if ((headerValue != null) && (!string.IsNullOrEmpty(headerValue)))
                {
                    request.Headers.Add(HeaderName, headerValue);
                }
                else
                {
                    IEnumerable<string> headerValues;
                    headerValues = objHeaderLoopIds as IEnumerable<string>;
                    if ((headerValues != null) && (headerValues.Count() > 0))
                    {
                        request.Headers.Add(HeaderName, headerValues.ToList());
                    }
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
