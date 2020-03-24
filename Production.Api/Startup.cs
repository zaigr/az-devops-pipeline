using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using Production.Api.Data;
using Production.Api.Exceptions.Handlers;
using Production.Api.Mapping;
using Production.Api.Middlewares;
using Production.Api.Models;
using Serilog;

namespace Production.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureLogging();

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddTransient<IExceptionHandler, EntryNotFoundExceptionHandler>();
            services.AddDbContext<AdventureWorksContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("AdventureWorks"));
            });

            services.AddApplicationInsightsTelemetry(Configuration["InstrumentationKey"]);

            services.AddControllers(opt =>
            {
                opt.EnableEndpointRouting = false;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Production", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddOData().Services.AddMvcCore(options =>
            {
                // workaround for https://github.com/OData/WebApi/issues/1177
                foreach (var formatter in options.OutputFormatters
                    .OfType<ODataOutputFormatter>()
                    .Where(it => !it.SupportedMediaTypes.Any()))
                {
                    formatter.SupportedMediaTypes.Add(
                        new MediaTypeHeaderValue("application/prs.mock-odata"));
                }

                foreach (var formatter in options.InputFormatters
                    .OfType<ODataInputFormatter>()
                    .Where(it => !it.SupportedMediaTypes.Any()))
                {
                    formatter.SupportedMediaTypes.Add(
                        new MediaTypeHeaderValue("application/prs.mock-odata"));
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Production");
                c.RoutePrefix = "swagger";
            });

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.Select().Filter().Count().MaxTop(100);
                routeBuilder.MapODataServiceRoute("api", "api", GetEdmModel());

                routeBuilder.EnableDependencyInjection();
            });
        }

        private IEdmModel GetEdmModel()
        {
            // comment
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Product>("Products");

            return odataBuilder.GetEdmModel();
        }

        private void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.ApplicationInsights(TelemetryConverter.Traces)
                .CreateLogger();
        }
    }
}
