using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace DistributedLoopDetector
{
    public partial class LoopDetectorHandler 
    {
        /// <summary>
        /// IHttpContextAccessor for asp.net
        /// </summary>
        private IHttpContextAccessor? _contextAccessor;

        /// <summary>
        /// Constructor for the DI
        /// </summary>
        /// <param name="contextAccessor">Required context accessor</param>
        public LoopDetectorHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            Items = contextAccessor?.HttpContext?.Items;
        }

        /// <summary>
        /// HttpContext's Items 
        /// </summary>
        /// <returns>
        /// HttpContext's Items 
        /// </returns>
        private IDictionary<object, object>? GetItems()
        {
            return _contextAccessor?.HttpContext?.Items != null?_contextAccessor.HttpContext.Items:items;
        }


    }
}
