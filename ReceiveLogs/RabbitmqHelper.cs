using RabbitMQ.Client;

namespace ReceiveLogs
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
                VirtualHost = "/vhost_mmr"
                //Endpoint = new AmqpTcpEndpoint(uri)
            };
            var connection = factory.CreateConnection();
            return connection;
        }
    }
}
