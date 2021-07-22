using Demo.DDD.WithEFCore.API.Library;
using Demo.DDD.WithEFCore.Data;
using Demo.DDD.WithEFCore.Data.Repositories;
using Demo.DDD.WithEFCore.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.DDD.WithEFCore.API
{
    [ExcludeFromCodeCoverage]
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
            services.AddAutoMapper(typeof(Startup));

            /*
             * AddNewtonsoftJson replaces the System.Text.Json based input and output formatters used for formatting 
             * all JSON content. To add support for JsonPatch using Newtonsoft.Json, while leaving the other formatters 
             * unchanged, update the project's Startup.ConfigureServices as follows:
             */
            services.AddControllers(options => {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            }).AddNewtonsoftJson();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;

                // to let the clients of the API know all supported versions
                config.ReportApiVersions = true;

                config.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo.DDD.WithEFCore.API", Version = "v1" });
                // c.DocumentFilter<JsonPatchDocumentFilter>();
            });

            // services.AddSwaggerExamplesFromAssemblyOf<JsonPatchUserRequestExample>();

            services.AddDbContext<OrderDbContext>(options => {
                options.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=DemoOwnedEntity;Trusted_Connection=True;MultipleActiveResultSets=true");
                options.LogTo(Console.WriteLine);
                options.EnableSensitiveDataLogging(true); ;
                options.EnableDetailedErrors(true);
            });

            // services.AddScoped<IRepository<Order>, GenericRepository<Order, OrderDbContext>>();
            services.AddScoped<IRepository<Order>, OrderRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo.DDD.WithEFCore.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddLogging()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }
    }
}
