using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ReceiveLogs2Direct
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var conn = RabbitmqHelper.GetConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    var qName = channel.QueueDeclare().QueueName;
                    channel.ExchangeDeclare("direct_logs", ExchangeType.Direct);
                    channel.QueueBind(qName, "direct_logs", "info", null);
                    channel.QueueBind(qName, "direct_logs", "error", null);
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        channel.BasicAck(ea.DeliveryTag, false);
                        Console.WriteLine($"[reveive msg:]{message}");
                    };
                    channel.BasicConsume(qName, false, consumer);
                    Console.WriteLine("press [enter] to exit");
                    Console.ReadLine();
                }
            }

        }
    }
}
