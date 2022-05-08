using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPS.DistributedLoopDetectorFunc
{
    internal static class FunctionContextExtensions
    {
        public static HttpRequestData GetHttpRequestData(this FunctionContext functionContext)
        {
            try
            {
                KeyValuePair<Type, object> keyValuePair = functionContext.Features.SingleOrDefault(f => f.Key.Name == "IFunctionBindingsFeature");
                object functionBindingsFeature = keyValuePair.Value;
                Type type = functionBindingsFeature.GetType();
                var inputData = type.GetProperties().Single(p => p.Name == "InputData").GetValue(functionBindingsFeature) as IReadOnlyDictionary<string, object>;
                return inputData?.Values.SingleOrDefault(o => o is HttpRequestData) as HttpRequestData;
            }
            catch
            {
                return null;
            }
        }

        public static void SetResponseData(this FunctionContext functionContext, HttpResponseData responseData)
        {
            var feature = functionContext.Features.FirstOrDefault(f => f.Key.Name == "IFunctionBindingsFeature").Value;
            if (feature == null) throw new Exception("Required binding feature is not present.");
            var pinfo = feature.GetType().GetProperty("InvocationResult");
            pinfo.SetValue(feature, responseData);
        }
    }
}
