using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PhishProtector.Windows
{
    public class Startup
    {
        /// <summary>
        /// The Startup constructor accepts an IConfiguration instance. This instance is automatically provided by the ASP.NET Core dependency injection framework. 
        /// </summary>
        /// <param name="configuration">The configuration instance represents the application's configuration.
        /// This configuration can include settings from appsettings.json, environment variables, command line arguments, etc.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Represents the application's configuration. Can be used to retrieve configuration settings.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method is called by the runtime to add services to the container.
        /// This method allows you to configure the services that will be used by your application.
        /// </summary>
        /// <param name="services">Service collection to add services to.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding MVC services to the services container
            // SetCompatibilityVersion is used to set the compatibility version of MVC, which can affect the behavior of MVC services
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        /// <summary>
        /// This method is called by the runtime to configure the HTTP request pipeline.
        /// The pipeline defines how the application responds to HTTP requests.
        /// </summary>
        /// <param name="app">Application builder used to configure request pipeline.</param>
        /// <param name="env">Hosting environment information.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Checking if the application is in development mode
            // In development mode, detailed error pages are used
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Adding MVC to the request execution pipeline
            // This allows the application to respond to HTTP requests with MVC views or controllers
            app.UseMvc();
        }
    }


}
