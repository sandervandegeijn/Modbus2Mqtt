using System.Threading.Tasks;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Modbus2Mqtt.BackgroundServices;
using Modbus2Mqtt.Infrastructure;


namespace Modbus2Mqtt
{
    class Program
    {

        static async Task Main(string[] args)
        {
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
                    services.AddLogging(logging =>
                    {
                        logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                        logging.AddConsole(c =>
                        {
                            c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                        });
                        logging.AddDebug();
                    });
                });

            await builder.RunConsoleAsync();
        }
    }
}