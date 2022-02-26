namespace Demo.DDD.WithEFCore.API
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            /*
             * AddNewtonsoftJson replaces the System.Text.Json based input and output formatters used for formatting 
             * all JSON content. To add support for JsonPatch using Newtonsoft.Json, while leaving the other formatters 
             * unchanged, update the project's Startup.ConfigureServices as follows:
             */
            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
                options.ReturnHttpNotAcceptable = true;
            }).AddXmlDataContractSerializerFormatters()
                .AddNewtonsoftJson();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                // to let the clients of the API know all supported versions
                config.ReportApiVersions = true;

                //options.Conventions.Controller<ValuesController>().HasApiVersion(1, 0); /* OR [ApiVersion("1.0")] */
                //options.Conventions.Controller<ValuesController>().HasDeprecatedApiVersion(1, 0).HasApiVersion(1, 1).HasApiVersion(2, 0).Action(c => c.Get1_0()).MapToApiVersion(1, 0).Action(c => c.Get1_1()).MapToApiVersion(1, 1).Action(c => c.Get2_0()).MapToApiVersion(2, 0);

                config.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new QueryStringApiVersionReader("v"),
                    new UrlSegmentApiVersionReader()
                    );

            }).AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(c =>
            {
                // add a custom operation filter which sets default values
                c.OperationFilter<SwaggerDefaultValues>();

                // c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo.DDD.WithEFCore.API", Version = "v1" });
                // c.DocumentFilter<JsonPatchDocumentFilter>();

                // integrate xml comments                
                c.IncludeXmlComments(XmlCommentsFilePath());

                /*
                 options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,
                            },
                            new List<string>()
                        }
                    });
                 */
            });
            services.ConfigureOptions<ConfigureSwaggerOptions>();

            services.AddProblemDetails(options =>
            {
                // Control when an exception is included
                options.IncludeExceptionDetails = (ctx, ex) =>
                {
                    // Fetch services from HttpContext.RequestServices
                    var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                    return env.IsDevelopment() || env.IsStaging();
                };
            });

            // services.AddSwaggerExamplesFromAssemblyOf<JsonPatchUserRequestExample>();

            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=DemoOwnedEntity;Trusted_Connection=True;MultipleActiveResultSets=true");
                options.LogTo(Console.WriteLine);
                options.EnableSensitiveDataLogging(true); ;
                options.EnableDetailedErrors(true);
            });

            // services.AddScoped<IRepository<Order>, GenericRepository<Order, OrderDbContext>>();
            services.AddScoped<IRepository<Order>, OrderRepository>();

            services.AddScoped<IDiscountService, NewYearDiscountService>();
            services.AddScoped<IDiscountService, SpecialDiscountService>();

            // Provides a mapping between file exts and MIME types
            services.AddSingleton<FileExtensionContentTypeProvider>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(
                    options =>
                    {
                        // build a swagger endpoint for each discovered API version
                        foreach (var groupName in provider.ApiVersionDescriptions.Select(description => description.GroupName))
                        {
                            options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName.ToUpperInvariant());
                        }
                    });
            }

            // app.UseProblemDetails();

            app.UseExceptionHandler(appBuilder =>
            {

            });

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

        private static string XmlCommentsFilePath()
        {
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
            return Path.Combine(basePath, fileName);
        }
    }
}
