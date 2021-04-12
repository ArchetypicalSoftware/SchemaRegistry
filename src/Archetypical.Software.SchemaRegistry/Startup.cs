using System;
using System.IO;
using Archetypical.Software.SchemaRegistry.Data.Cosmos;
using Archetypical.Software.SchemaRegistry.Data.Sqlite;
using Archetypical.Software.SchemaRegistry.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Archetypical.Software.SchemaRegistry.Shared;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;

using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Archetypical.Software.SchemaRegistry
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
#if DEBUG
                .AddRazorRuntimeCompilation()
#endif
                ;

            services.AddSqlite();
            //services.AddCosmos(Configuration);
            services
                .AddTransient<ISchemaValidator, JsonSchemaValidator>()
                .AddTransient<ISchemaValidator, AvroSchemaValidator>()
                .AddTransient<ISchemaValidator, Proto3SchemaValidator>()
                .AddTransient<ISchemaValidator, XsdSchemaValidator>();

            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("0.1", new OpenApiInfo
                    {
                        Version = "0.1",
                        Title = "Cloud Native Data Schema Registry",
                        Description = "Cloud Native Data Schema Registry - Implemented by Archetypical Software",
                        Contact = new OpenApiContact()
                        {
                            Name = "Archetypical Software",
                            Url = new Uri("https://archetypical.software/products/schemaregistry"),
                            Email = "development@archetypical.software"
                        },
                        TermsOfService = new Uri("https://archetypical.software/#contact")
                    });
                    c.CustomSchemaIds(type => type.FullName);
                    c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{_env.ApplicationName}.xml");

                    // Include DataAnnotation attributes on Controller Action parameters as Swagger validation rules (e.g required, pattern, ..)
                    // Use [ValidateModelState] on Actions to actually validate it in C# as well!
                    c.OperationFilter<GeneratePathParamsValidationFilter>();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSerilogRequestLogging();

            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger-original.json", "Cloud Native Data Schema Registry Original");
            });

            #region snippet_route

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            #endregion snippet_route
        }
    }
}