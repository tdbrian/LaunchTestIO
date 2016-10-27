using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using LaunchTestIO.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ng2Webpack;
using Owin;
using Sector7G.config;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;

namespace LaunchTestIO
{
    public class Startup
    {
        public IContainer ApplicationContainer { get; set; }

        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            services.AddWebpack();

            // Add Autofac
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<InjectionModule>();
            containerBuilder.Populate(services);

            // Register your SignalR hubs.
            containerBuilder.RegisterHubs(Assembly.GetExecutingAssembly());

            ApplicationContainer = containerBuilder.Build();
            GlobalHost.DependencyResolver = new AutofacDependencyResolver(ApplicationContainer);
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseAppBuilder(appBuilder =>
            {
                appBuilder.Properties["host.AppName"] = "LaunchTestIO";
                appBuilder.UseAutofacMiddleware(ApplicationContainer);
                appBuilder.MapSignalR();
            });

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            if (env.IsDevelopment())
            {
                app.UseWebpack("webpack.config.js", new Ng2WebpackOptions
                {
                    DevServerOptions = new Ng2WebpackDevServerOptions("localhost", 8080),
                    EnableHotLoading = false,
                    OutputFileNames = new List<string> { "polyfills.js", "vendor.js", "app.js" }
                });
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            appLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());
        }
    }
}
