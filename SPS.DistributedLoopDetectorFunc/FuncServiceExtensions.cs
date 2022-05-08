using DistributedLoopDetector;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPS.DistributedLoopDetectorFunc
{
    public static class FuncServiceExtensions
    {
        /// <summary>
        /// Add everything that you need to detect a distributed loop
        /// </summary>
        /// <param name="services">Current service collection</param>
        /// <returns>modified service collection</returns>
        public static IServiceCollection AddFuncDistributedLoopDetector(this IServiceCollection services)
        {
            services.AddHttpContextAccessor() //adds http context accessor
                .AddLogging()
                //.AddScoped<LoopDetectorHandler>()
                .AddTransient<LoopDetectorHandler>() //register the handler to add current loop context 
                .AddSingleton<LoopDetectStackInstance>()
                .ConfigureAll<HttpClientFactoryOptions>(options => //its add loopid context inside the header in every HttpClient Calls
                {
                    options.HttpMessageHandlerBuilderActions.Add(builder =>
                    {
                        builder.AdditionalHandlers.Add(builder.Services.GetService<LoopDetectorHandler>()); //.GetRequiredService<LoopDetectorHandler>());
                    });
                });
            services.AddHttpClient();
            return services;
        }

        public static void UseDistributedLoopDetector(this IFunctionsWorkerApplicationBuilder workerApplication)
        {
            workerApplication.UseWhen<FuncLoopDetectorMiddelware>((context) =>
            {
                // We want to use this middleware only for http trigger invocations.
                return context.FunctionDefinition.InputBindings.Values
                              .First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";
            });
        }
    }
}
