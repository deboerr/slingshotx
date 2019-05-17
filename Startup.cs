using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using slingshotx.Services;
using Swashbuckle.AspNetCore.Swagger;
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
            var connStr = Configuration.GetConnectionString("DefaultConnection");
            if (connStr == null)
                throw new ArgumentNullException("Connection string is null");

            services.AddTransient(provider => new MeetingService(connStr));
            services.AddTransient(provider => new RaceService(connStr));
            services.AddTransient(provider => new RunnerService(connStr));
            services.AddTransient(provider => new ScratchingService(connStr));
            services.AddTransient(provider => new PodService(connStr));
            services.AddTransient(provider => new UserService(connStr));

            services.AddMvc();
            services.AddOptions();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "SlingshotX API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SlingshotX API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMvc();
        }
    }
}
