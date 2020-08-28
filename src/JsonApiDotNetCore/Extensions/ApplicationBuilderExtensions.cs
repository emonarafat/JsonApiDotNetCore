using System;
using JsonApiDotNetCore.Builders;
using JsonApiDotNetCore.Internal;
using JsonApiDotNetCore.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace JsonApiDotNetCore
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Registers the JsonApiDotNetCore middleware.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="configureMvcOptions">Configure .NET Core MVC options</param>
        /// <example>
        /// The code below is the minimal that is required for proper activation,
        /// which should be added to your Startup.Configure method.
        /// <code><![CDATA[
        /// app.UseRouting();
        /// app.UseJsonApi();
        /// app.UseEndpoints(endpoints => endpoints.MapControllers());
        /// ]]></code>
        /// </example>
        public static void UseJsonApi(this IApplicationBuilder builder, Action<MvcOptions> configureMvcOptions = null)
        {
            var jsonApiApplicationBuilder =  builder.ApplicationServices.GetRequiredService<IJsonApiApplicationBuilder>();
            jsonApiApplicationBuilder.ConfigureMvcOptions = options =>
            {
                options.Conventions.Insert(0, builder.ApplicationServices.GetRequiredService<IJsonApiRoutingConvention>());
                configureMvcOptions?.Invoke(options);
            };
            
            builder.UseMiddleware<JsonApiMiddleware>();
        }
    }
}
