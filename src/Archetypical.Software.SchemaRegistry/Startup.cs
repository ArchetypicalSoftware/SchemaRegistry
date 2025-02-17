using System;
using System.IO;
using Archetypical.Software.SchemaRegistry.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Archetypical.Software.SchemaRegistry.Shared;
using Archetypical.Software.SchemaRegistry.Shared.Interfaces;
using Archetypical.Software.Vega.Api.Abstractions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.OpenApi.Models;

namespace Archetypical.Software.SchemaRegistry
{
    public static class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
                options.HandleSameSiteCookieCompatibility();
            });

            // Configuration to sign-in users with Azure AD B2C
            //services.AddMicrosoftIdentityWebAppAuthentication(configuration, "AzureAdB2C")
            //    .EnableTokenAcquisitionToCallDownstreamApi()
            //    .AddDistributedTokenCaches();

            services.AddControllersWithViews()
                //.AddMicrosoftIdentityUI()

#if DEBUG
                .AddRazorRuntimeCompilation()
#endif
                ;
            services.AddRazorPages();

            //services.AddCosmos(Configuration);
            services
                .AddTransient<ISchemaValidator, JsonSchemaValidator>()
                .AddTransient<ISchemaValidator, AvroSchemaValidator>()
                .AddTransient<ISchemaValidator, Proto3SchemaValidator>()
                .AddTransient<ISchemaValidator, OpenApiSchemaValidator>()
                .AddTransient<ISchemaValidator, XsdSchemaValidator>();
            Archetypical.Software.Vega.Api.Abstractions.VegaApiOptions vegaOptions = new VegaApiOptions();
            configuration.Bind(vegaOptions);

            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
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
                    //c.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{env.ApplicationName}.xml");

                    // Include DataAnnotation attributes on Controller Action parameters as Swagger validation rules (e.g required, pattern, ..)
                    // Use [ValidateModelState] on Actions to actually validate it in C# as well!
                    c.OperationFilter<GeneratePathParamsValidationFilter>();
                });

            //Configuring appsettings section AzureAdB2C, into IOptions
            services.AddOptions();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static WebApplication Configure(this WebApplication app, IWebHostEnvironment env)
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

            #region snippet_route

            app.UseRouting().UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            #endregion snippet_route

            Archetypical.Software.Vega.Api.Abstractions.VegaApiApplication.UseVegaApiDefaults(app);
            return app;
        }
    }
}