using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPS.DistributedLoopDetectorFunc
{
    /// <summary>
    /// Extension to read\write Request and Response of Function
    /// </summary>
    internal static class FunctionContextExtensions
    {
        /// <summary>
        /// Get HttpRequest from function context
        /// </summary>
        /// <param name="functionContext">context to extend</param>
        /// <returns>Function's HttpRequest</returns>
        public static HttpRequestData? GetHttpRequestData(this FunctionContext functionContext)
        {
            try
            {
                KeyValuePair<Type, object> keyValuePair = functionContext.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
                object functionBindingsFeature = keyValuePair.Value;
                Type type = functionBindingsFeature.GetType();
                var inputData = type.GetProperties().Single(p => p.Name == "InputData").GetValue(functionBindingsFeature) as IReadOnlyDictionary<string, object>;
                if (inputData?.Values != null)
                {
                    return inputData.Values.SingleOrDefault(o => o is HttpRequestData) as HttpRequestData;
                }
                else
                {
                    return null;
                }

            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Set Response to Function
        /// </summary>
        /// <param name="functionContext">context to extend</param>
        /// <param name="responseData">Response for Function</param>
        /// <exception cref="Exception">Throw Exception if IFunctionBindingsFeature not found</exception>
        public static void SetResponseData(this FunctionContext functionContext, HttpResponseData responseData)
        {
            var feature = functionContext.Features.FirstOrDefault(f => f.Key.Name == "IFunctionBindingsFeature").Value;
            if (feature == null) throw new Exception("Required binding feature is not present.");
            var pinfo = feature.GetType().GetProperty("InvocationResult");
            if (pinfo != null)
            {
                pinfo.SetValue(feature, responseData);
            }
        }
    }
}
