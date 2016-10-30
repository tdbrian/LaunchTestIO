using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ng2Webpack;
using LaunchTestIO.Backend.Authentication;
using LaunchTestIO.Backend.Users;
using LaunchTestIO.Config.Database;
using Newtonsoft.Json;

namespace LaunchTestIO
{
    public class Startup
    {
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
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            // Register identity framework services and also Mongo storage. 
            services.AddIdentityWithMongoStores(Configuration.GetConnectionString("DefaultConnection"))
                .AddDefaultTokenProviders();

            // SignalR options
            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
            });

            // Camelcase serializations with custom contract resolver
            var settings = new JsonSerializerSettings { ContractResolver = new SignalRContractResolver() };
            var serializer = JsonSerializer.Create(settings);
            services.Add(new ServiceDescriptor(typeof(JsonSerializer), provider => serializer, ServiceLifetime.Transient));

            services.AddMvc();

            services.AddWebpack();

            services.AddTransient<IEmailSender, AuthenticationMessageService>();
            services.AddTransient<ISmsSender, AuthenticationMessageService>();
            services.AddTransient<ILaunchTestIoContext, LaunchTestIoMongoContext>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IUsersDatastore, UsersMongoDatastore>();

            // var userService = ApplicationContainer.Resolve<IUsersService>();
            // userService.PopulateDefaultAdmin();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // Cookie settings
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromDays(150);
                options.Cookies.ApplicationCookie.LoginPath = "/Account/LogIn";
                options.Cookies.ApplicationCookie.LogoutPath = "/Account/LogOff";

                // User settings
                options.User.RequireUniqueEmail = true;

                // Signin settings
                options.SignIn.RequireConfirmedEmail = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIdentity();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseWebSockets();

            app.UseSignalR();

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
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("spa-fallback", "{*anything}", new { controller = "Home", action = "Index" });
                routes.MapWebApiRoute("defaultApi", "api/{controller}/{id?}");
            });
        }
    }
}
