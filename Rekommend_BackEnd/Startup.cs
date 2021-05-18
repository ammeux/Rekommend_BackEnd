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
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Rekommend_BackEnd.Authorization;
using Microsoft.AspNetCore.Authorization;

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

            // Add headers to use ETags for cacheing
            services.AddHttpCacheHeaders((expirationModelOptions) =>
            {
                expirationModelOptions.MaxAge = 30;
                expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
            },
            (validationModelOptions) =>
            {
                validationModelOptions.MustRevalidate = false;
            });

            // Add controllers
            services.AddControllers(setupAction =>
            {
                // By default, return not acceptable mediaType
                setupAction.ReturnHttpNotAcceptable = true;
            })
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    // Creation of a problem details object
                    var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext,
                        context.ModelState);

                    // Add additional info not added by default
                    problemDetails.Detail = "See the error fields for more detail.";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                    // Find out which status to use
                    var actionExecutingContext = context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                    // If there are modelstate errors, deal with validation errors
                    if((context.ModelState.ErrorCount >0) && (actionExecutingContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                    {
                        problemDetails.Type = "modelValidationProblem";
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Title = "Validation errors have been raised";

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };

                    // If one of the arguments was not correctly found / could not be parsed we are dealing with null/unparsable input
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "One or more errors on input occured";
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
             })
            //used to render CamelCase properties
            .AddNewtonsoftJson(
            setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
            
            // Used to create Url in filterAttributes
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //Add scoped on Repository as method are called asynchronously
            services.AddScoped<IRekommendRepository, RekommendRepository>();
            services.AddScoped<IAuthorizationHandler, MustBeARecruiterHandler>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            // Add Cors Policies
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
                });
            });

            // Identity Server 4 authentication
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:44353";
                    //options.RequireHttpsMetadata = false;
                    options.ApiName = "rekommendapi";
                    //options.CacheDuration = TimeSpan.FromMinutes(10); // Default
                });

            // Identity Server 4 authorization
            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                    "MustBeARecruiter",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.AddRequirements(new MustBeARecruiterRequirement());
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Default exception handler that can be configured
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }

            app.UseHttpsRedirection();

            //app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
