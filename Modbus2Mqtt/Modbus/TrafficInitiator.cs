using System.Collections.Generic;
using System.Threading.Tasks;
using Modbus2Mqtt.Infrastructure.Configuration;
using NLog;

namespace Modbus2Mqtt.Modbus
{
    public class TrafficInitiator
    {
        private readonly Configuration _configuration;
        private readonly ModbusCommunicator _modbusCommunicator;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public TrafficInitiator(Configuration configuration, ModbusCommunicator modbusCommunicator)
        {
            _configuration = configuration;
            _modbusCommunicator = modbusCommunicator;
        }

        public void Start()
        {
            var tasks = new List<Task>();
            foreach (var slave in _configuration.Slave)
            {
                tasks.Add(Task.Factory.StartNew(() => { StartCommunication(slave); }));
            }
        }

        private async void StartCommunication(Slave slave)
        {
            Logger.Info("Starting task for " + slave.Name);
            while (true)
            {
                _modbusCommunicator.RequestForSlave(slave);
                Logger.Info("Polling for " + slave.Name);
                await Task.Delay(slave.PollingInterval);
            }
        }
    }
}