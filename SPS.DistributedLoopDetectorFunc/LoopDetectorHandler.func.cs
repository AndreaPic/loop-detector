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
        private HttpContext _context;

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

        private IDictionary<object, object> GetItems()
        {
            return items;
        }


    }
}
