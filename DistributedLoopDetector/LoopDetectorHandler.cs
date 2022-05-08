using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace DistributedLoopDetector
{
    /// <summary>
    /// Add the current loop id context to the header for the http calls
    /// </summary>
    public partial class LoopDetectorHandler : DelegatingHandler
    {
        private IDictionary<object, object> items;
        private IDictionary<object, object> Items 
        { 
            get
            {
                return GetItems();
            }
            set
            {
                items = value;
            }
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
            object? objHeaderLoopIds;
            if (Items==null)
            {
                throw new ArgumentNullException("Items");
            }
            objHeaderLoopIds = GetLoopIdFromItemsAddToHeader(request.Headers, Items);
            HttpResponseMessage? ret = null;
            try
            {
                ret = await base.SendAsync(request, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (Items != null)
                    {
                        Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (Items != null)
                {
                    Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (Items != null)
                {
                    Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        internal static object? GetLoopIdFromItemsAddToHeader(HttpRequestHeaders requestHeaders, IDictionary<object, object> items)
        {
            object? objHeaderLoopIds = null;
            if ((items != null) && (requestHeaders!=null))
            {

                bool aquired = false;
                int count = 0;
                do
                {
                    aquired = items.TryGetValue(LoopDetectorHandler.HeaderName, out objHeaderLoopIds);
                    count++;
                }
                while (!aquired && count < 3);

                if (aquired && objHeaderLoopIds != null)
                {
                    string? headerValue;
                    headerValue = objHeaderLoopIds as string;
                    if ((headerValue != null) && (!string.IsNullOrEmpty(headerValue)))
                    {
                        System.Diagnostics.Debug.WriteLine($"TO HEADER: {HeaderName}-{headerValue}");
                        requestHeaders.Add(HeaderName, headerValue);
                    }
                    else
                    {
                        IEnumerable<string>? headerValues;
                        headerValues = objHeaderLoopIds as IEnumerable<string>;
                        if ((headerValues != null) && (headerValues.Count() > 0))
                        {
                            foreach (string item in headerValues)
                            {
                                requestHeaders.Add(HeaderName, item);
                            }
                        }
                    }
                }
            }

            return objHeaderLoopIds;
        }
    }
}
