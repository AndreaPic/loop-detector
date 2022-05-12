using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoopDetector
{
    /// <summary>
    /// Extension to detect a distributed loop
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add everything that you need to detect a distributed loop
        /// </summary>
        /// <param name="services">Current service collection</param>
        /// <returns>modified service collection</returns>
        public static IServiceCollection AddDistributedLoopDetector(this IServiceCollection services)
        {
            services.AddHttpContextAccessor() //adds http context accessor
                .AddLogging()
                .AddTransient<LoopDetectorHandler>() //register the handler to add current loop context 
                .AddSingleton<LoopDetectStackInstance>()
                .ConfigureAll<HttpClientFactoryOptions>(options => //its add loopid context inside the header in every HttpClient Calls
                {
                    options.HttpMessageHandlerBuilderActions.Add(builder =>
                    {
                        builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<LoopDetectorHandler>());
                    });
                }).AddMvc(options =>
                {
                    options.Filters.Add<LoopDetectResourceFilter>(); //if detect a loop it returns http error
                    options.Filters.Add<LoopDetectActionFilter>(); //add and remove current loop context for the current action
                });
            services.AddHttpClient();
            return services;
        }

        /// <summary>
        /// Use to use DistributedCache instead of local memory
        /// </summary>
        /// <param name="app">application builder</param>
        /// <param name="applicationName">application name</param>
        /// <returns>Configured applicaton builder</returns>
        public static IApplicationBuilder UseDistributedCacheForLoopDetector(this IApplicationBuilder app, string applicationName)
        {
            var cache = app.ApplicationServices.GetService<IDistributedCache>();
            if (cache == null)
            {
                throw new NullReferenceException($"{ typeof(IDistributedCache).Name } is null or not registered");
            }
            else
            {
                LoopDetectStack.Instance.SetDistributedCache(cache, applicationName);
            }
            return app;
        }

    }
}
