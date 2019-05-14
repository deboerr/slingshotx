using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using slingshotx.Services;
using System;

namespace slingshotx
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connStr = Configuration.GetConnectionString("Danube");
            if (connStr == null)
                throw new ArgumentNullException("Connection string is null");

            services.AddTransient(provider => new MeetingService(connStr));
            // services.AddTransient(provider => new RaceService(connStr));
            // services.AddTransient(provider => new RunnerService(connStr));        
            // services.AddTransient(provider => new PodService(connStr));
            // services.AddTransient(provider => new UserService(connStr));

            services.AddMvc();
            services.AddOptions();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
