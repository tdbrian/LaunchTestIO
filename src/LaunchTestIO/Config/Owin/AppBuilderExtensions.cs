using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Owin.Builder;
using Owin;

namespace LaunchTestIO.Config
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseAppBuilder(this IApplicationBuilder app, Action<IAppBuilder> configure)
        {
            app.UseOwin(addToPipeline =>
            {
                addToPipeline(next =>
                {
                    var appBuilder = new AppBuilder();
                    appBuilder.Properties["builder.DefaultApp"] = next;

                    configure(appBuilder);

                    return appBuilder.Build<Func<IDictionary<string, object>, Task>>();
                });
            });

            return app;
        }
    }
}
