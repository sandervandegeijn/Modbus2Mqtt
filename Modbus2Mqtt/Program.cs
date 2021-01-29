using System;
using EasyModbus;
using Lamar;
using MediatR;
using MediatR.Pipeline;
using Modbus2Mqtt.Infrastructure;
using Modbus2Mqtt.Infrastructure.Configuration;
using Modbus2Mqtt.Modbus;
using Modbus2Mqtt.Mqtt;
using MQTTnet.Client;
using NLog;

namespace Modbus2Mqtt
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        static void Main(string[] args)
        {
            Logger.Info("Starting application");
            
            var container = new Container(x =>
            {
                x.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Program));
                    _.WithDefaultConventions();
                    _.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    _.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    _.ConnectImplementationsToTypesClosing(typeof(IRequestExceptionAction<>));
                    _.ConnectImplementationsToTypesClosing(typeof(IRequestExceptionHandler<,,>));
                });
                x.For<IMediator>().Use<Mediator>().Transient();
                x.For<ServiceFactory>().Use(ctx => ctx.GetInstance);
                x.For<ModbusClient>().Use(s => ModbusclientFactory.GetModbusClient());
                x.For<Configuration>().Use(s => ConfigurationFactory.GetConfiguration());
                x.For<IMqttClient>().Use(s => s.GetInstance<MqttConfigFactory>().GetMqttClient().Result).Singleton();
            });

            var trafficInitiator = container.GetInstance<TrafficInitiator>();
            trafficInitiator.Start();
            var mqttListener = container.GetInstance<MqttListener>();
            mqttListener.Start();
            Console.Read();
        }
    }
}