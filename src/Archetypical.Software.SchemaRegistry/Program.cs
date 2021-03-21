using System;
using System.Diagnostics;
using System.Reflection;
using Archetypical.Software.SchemaRegistry.Shared.Data;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Archetypical.Software.SchemaRegistry
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    services.GetRequiredService<Context>().Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .MinimumLevel.ControlledBy(new LoggingLevelSwitch(LogEventLevel.Information))
                        .Enrich.FromLogContext()
                        .Enrich.WithEnvironmentUserName()
                        .Enrich.WithMachineName()
                        .Enrich.WithProcessId()
                        .Enrich.WithProcessName()
                        .Enrich.WithAssemblyName()
                        .Enrich.WithAssemblyVersion()
                        .Enrich.WithProperty("ApplicationName", "Archetypical.Software.SchemaRegistry");

                    // Standard console logging when debugging locally
                    if (hostingContext.HostingEnvironment.IsDevelopment() && Debugger.IsAttached)
                    {
                        loggerConfiguration.WriteTo.Console();
                    }
                    else
                    {
                        loggerConfiguration.WriteTo.Console(new RenderedCompactJsonFormatter());
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .ConfigureKestrel(options =>
                        {
                            options.AddServerHeader = false;
                            options.AllowResponseHeaderCompression = true;
                        });
                });
    }
}