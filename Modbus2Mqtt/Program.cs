using System.Threading.Tasks;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.BackgroundServices;
using Modbus2Mqtt.Infrastructure;
using Modbus2Mqtt.Modbus;
using NLog;

namespace Modbus2Mqtt
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        static async Task Main(string[] args)
        {
            Logger.Info("Starting application");
            
            var builder = new HostBuilder()
                .UseLamar()
                .ConfigureContainer<ServiceRegistry>((context, services) =>
                {
                    services.IncludeRegistry<Registry>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddHostedService<ModbusRequestQueueBackgroundService>();
                    services.AddHostedService<ModbusRequestPollerBackgroundService>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }
    }
}