using RabbitMQ.Client;
using System;

namespace ReceiveLogsDirect
{
    public static class RabbitmqHelper
    {
        public static IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                UserName = "admin",
                Password = "admin",
                Port = 5672,
                HostName = "192.168.150.129",
                RequestedHeartbeat = 0,
                VirtualHost = "/vhost_mmr",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = new TimeSpan(3000)
                //Endpoint = new AmqpTcpEndpoint(uri)
            };
            var connection = factory.CreateConnection();
            return connection;
        }
    }
}
