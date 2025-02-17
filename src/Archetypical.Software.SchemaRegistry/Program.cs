using Archetypical.Software.Vega.Api.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Archetypical.Software.SchemaRegistry
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args);
            host.Services.ConfigureServices(host.Configuration, host.Environment);
            host.Build().Configure(host.Environment).Run();
        }

        public static WebApplicationBuilder CreateHostBuilder(string[] args) =>
            VegaApiApplication.CreateVegaApi(options =>
            {
                options.ServiceName = "Archetypical.Software.SchemaRegistry";
            }, args);
    }
}