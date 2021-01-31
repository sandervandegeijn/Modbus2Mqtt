using EasyModbus;
using Lamar;
using MediatR;
using MediatR.Pipeline;
using Modbus2Mqtt.Infrastructure.YmlConfiguration;
using MQTTnet.Client;

namespace Modbus2Mqtt.Infrastructure
{
    public class Registry : ServiceRegistry
    {
        public Registry()
        {
            Scan(x =>
            {
                x.WithDefaultConventions();
                x.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                x.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                x.ConnectImplementationsToTypesClosing(typeof(IRequestExceptionAction<>));
                x.ConnectImplementationsToTypesClosing(typeof(IRequestExceptionHandler<,,>));
                x.TheCallingAssembly();
            });
            For(typeof(IPipelineBehavior<,>)).Add(typeof(RequestExceptionProcessorBehavior<,>));
            For(typeof(IPipelineBehavior<,>)).Add(typeof(RequestExceptionActionProcessorBehavior<,>));
            For(typeof(IPipelineBehavior<,>)).Add(typeof(RequestPreProcessorBehavior<,>));
            For(typeof(IPipelineBehavior<,>)).Add(typeof(RequestPostProcessorBehavior<,>));
            For<IMediator>().Use<Mediator>().Transient();
            For<ServiceFactory>().Use(ctx => ctx.GetInstance);
            For<ModbusClient>().Use(s => ModbusclientFactory.GetModbusClient());
            For<YmlConfiguration.Configuration.Configuration>().Use(s => ConfigurationFactory.GetConfiguration());
            For<IMqttClient>().Use(s => s.GetInstance<MqttConfigFactory>().GetMqttClient().Result).Singleton();
        }
    }
}