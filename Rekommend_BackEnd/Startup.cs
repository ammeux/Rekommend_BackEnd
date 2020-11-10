using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rekommend_BackEnd.DbContexts;
using Rekommend_BackEnd.Repositories;
using Rekommend_BackEnd.Services;
using System.IO;

namespace Rekommend_BackEnd
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup()
        {
            // Build own configuration to add Environment variable connection string
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables("MYASPNETCORE_");

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add dbContext
            services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(Configuration.GetConnectionString("RekommendDbConnection")));

            // Add controllers
            services.AddControllers();

            services.AddTransient<IEntityPropertiesService, EntityPropertiesService>();
            services.AddTransient<IRekommendRepository, RekommendRepository>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();
            
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
