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
                .AddTransient<LoopDetectorHandler>() //register the handler to add current loop context 
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
            return services;
        }
    }
}
