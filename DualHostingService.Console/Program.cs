using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using DualHostingService.Shared;

namespace DualHostingService.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureHostConfiguration(config =>
                {
                    config.SetBasePath(System.IO.Directory.GetCurrentDirectory());
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    config.AddJsonFile("appsettings.json");
                    config.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<ServiceConfig>(context.Configuration.GetSection("Service"));
                    services.AddHostedService<PrintTextToConsoleService>();
                });

            return hostBuilder;
        }
    }
}
