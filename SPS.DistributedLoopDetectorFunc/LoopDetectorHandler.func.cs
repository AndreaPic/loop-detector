using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;

namespace DistributedLoopDetector
{
    public partial class LoopDetectorHandler
    {
        private readonly IHttpContextAccessor contextAccessor;

        /// <summary>Use the current invocation context, including when HttpClient handlers are pooled.</summary>
        public LoopDetectorHandler(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        /// <summary>Expose the invocation's items to outbound HTTP calls.</summary>
        public void SetFunctionContext(FunctionContext context)
        {
            contextAccessor.HttpContext ??= new DefaultHttpContext();
            contextAccessor.HttpContext.Items = (IDictionary<object, object?>)context.Items;
        }

        private IDictionary<object, object?>? GetItems() => contextAccessor.HttpContext?.Items ?? items;
    }
}
