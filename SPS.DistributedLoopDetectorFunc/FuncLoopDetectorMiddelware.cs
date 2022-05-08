using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DistributedLoopDetector;
using System.Net;
using Microsoft.Extensions.Logging;

namespace SPS.DistributedLoopDetectorFunc
{
    public class FuncLoopDetectorMiddelware : IFunctionsWorkerMiddleware
    {
        private LoopDetectorHandler handler;
        private LoopDetectStackInstance loopDetectStack;
        private IHttpContextAccessor _httpContextAccessor;
        //private ILogger _logger;
        public FuncLoopDetectorMiddelware(LoopDetectorHandler loopDetectorHandler, LoopDetectStackInstance loopDetectStackInstance, IHttpContextAccessor httpContextAccessor)
        {
            handler = loopDetectorHandler;
            loopDetectStack = loopDetectStackInstance;
            _httpContextAccessor = httpContextAccessor;
            //_logger = logger;
        }

        private FunctionExecutionDelegate LoopDetectedExecutionResult = new((ctx) => 
        { 
            var response = ctx.GetHttpResponseData();
            return Task.CompletedTask; 
        });


        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {

            if (_httpContextAccessor == null)
            {
                _httpContextAccessor = new HttpContextAccessor();
            }
            if (_httpContextAccessor.HttpContext == null)
            {
                _httpContextAccessor.HttpContext = new DefaultHttpContext();
            }

            handler.SetFunctionContext(context);

            var httpRequestData = await context.GetHttpRequestDataAsync();
            string? loopId = null;
            IEnumerable<string> objLoopId = null;

            string? path = context?.FunctionDefinition.Name; //context?.FunctionId;
            if ((path != null) && (context != null) && (loopDetectStack != null))
            {

                if (!httpRequestData.Headers.Contains(LoopDetectorHandler.HeaderName))
                {
                    loopId = Guid.NewGuid().ToString("N");
                    loopDetectStack.AddLoopDetectInfo(path, loopId);
                    context.Items.Add(LoopDetectorHandler.HeaderName, loopId);
                    _httpContextAccessor.HttpContext.Items.Add(LoopDetectorHandler.HeaderName, loopId);
                    httpRequestData.Headers.Add(LoopDetectorHandler.HeaderName, loopId);
                }
                else
                {

                    var headers = httpRequestData.Headers;
                    if ((headers != null) && (!string.IsNullOrEmpty(path)))
                    {
                        IEnumerable<string> headerValues = null;
                        bool acquired = false;
                        int count = 0;
                        do
                        {
                            acquired = headers.TryGetValues(LoopDetectorHandler.HeaderName, out headerValues);
                        } 
                        while (!acquired && count < 3);

                        bool loopDetected = headerValues.Any(item => loopDetectStack.LoopDetectInfoMatch(path, item));
                        if (loopDetected)
                        {
                            if (context?.FunctionDefinition?.Name != null)
                            {
                                ILogger logger = context.GetLogger<FuncLoopDetectorMiddelware>();
                                //var logger = context.HttpContext.RequestServices.GetService(typeof(ILogger)) as ILogger;
                                if (logger != null)
                                {
                                    //TODO: use resources
                                    logger.Log(LogLevel.Error, $"Distributed Loop Detected in: { context?.FunctionDefinition?.Name }");
                                }
                            }


                            var request = context.GetHttpRequestData();
                            if (request != null)
                            {
                                var resp = request.CreateResponse(HttpStatusCode.LoopDetected);
                                //await response.WriteAsJsonAsync(data);
                                //resp.StatusCode = statusCode;
                                context.SetResponseData(resp);
                            }
                            return;
                        }
                        else
                        {
                            int i = 0;
                            bool retrieved = false;
                            do
                            {
                                retrieved = httpRequestData.Headers.TryGetValues(LoopDetectorHandler.HeaderName, out objLoopId);
                                i++;
                            } 
                            while (!retrieved && i < 3);

                            ILogger logger = context.GetLogger<FuncLoopDetectorMiddelware>();
                            foreach (var item in objLoopId)
                            {
                                loopDetectStack.AddLoopDetectInfo(path, item);
                                logger.Log(LogLevel.Information, $"PROPAGATE: { item }");
                            }
                            context.Items.Add(LoopDetectorHandler.HeaderName, objLoopId);
                            _httpContextAccessor.HttpContext.Items.Add(LoopDetectorHandler.HeaderName, objLoopId);

                        }

                    }

                }
            }

            handler.SetFunctionContext(context);


            //try
            //{
            await next(context);

            if (context?.Items != null)
            {
                if (!context.Items.ContainsKey(HttpStatusCode.RequestTimeout.ToString()))
                {
                    //httpRequestData = await context.GetHttpRequestDataAsync();

                    if (path != null && loopId != null)
                    {
                        loopDetectStack.RemoveLoopDetectInfo(path, loopId);
                        if (context != null)
                        {
                            context.Items.Remove(LoopDetectorHandler.HeaderName);
                            _httpContextAccessor.HttpContext.Items.Remove(LoopDetectorHandler.HeaderName);
                        }
                        loopDetectStack.RemoveLoopDetectInfo(path, loopId);
                        loopId = null;
                    }
                }
            }
            //}
            //catch (TimeoutException ex) 
            //{
            //}
            //catch(TaskCanceledException ex)
            //{
            //}

            //if (context.Items.TryGetValue("functionitem", out object value) && value is string message)
            //{
            //    ILogger logger = context.GetLogger<MyCustomMiddleware>();

            //    logger.LogInformation("From function: {message}", message);
            //}
        }
    }
}
