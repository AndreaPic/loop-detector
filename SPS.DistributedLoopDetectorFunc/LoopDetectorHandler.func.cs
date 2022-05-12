using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoopDetector
{
    partial class LoopDetectorHandler
    {
        /// <summary>
        /// HttpContext created instead of Functions
        /// </summary>
        private HttpContext _context;

        /// <summary>
        /// Constructor that require context's IHttpContextAccessor
        /// </summary>
        /// <param name="contextAccessor">IHttpContextAccessor to use</param>
        public LoopDetectorHandler(IHttpContextAccessor contextAccessor)
        {
            if (contextAccessor.HttpContext == null)
            {
                contextAccessor.HttpContext = new DefaultHttpContext();
            }
            _context = contextAccessor.HttpContext;
            Items = contextAccessor.HttpContext.Items;
        }


        //public LoopDetectorHandler(FunctionContext context)
        //{
        //    Items = context.Items;
        //}

        /// <summary>
        /// Used to force context Items on Functions
        /// </summary>
        /// <param name="context"></param>
        public void SetFunctionContext(FunctionContext context)
        {
            if (_context == null)
            {
                _context = new DefaultHttpContext();
            }

            Items = context.Items;
            foreach (var item in Items)
            {
                if (!_context.Items.ContainsKey(item.Key))
                {
                    _context.Items.Add(item);
                }
                else
                {
                    _context.Items.Remove(item.Key);
                    _context.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// HttpContext's Items 
        /// </summary>
        /// <returns>
        /// HttpContext's Items 
        /// </returns>
        private IDictionary<object, object>? GetItems()
        {
            return items;
        }


    }
}
