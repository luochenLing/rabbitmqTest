using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ReceiveLogsDirect
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var conn = RabbitmqHelper.GetConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.ExchangeDeclare("direct_logs", ExchangeType.Direct);
                    var qName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(queue: qName, exchange: "direct_logs", routingKey: "error", null);
                    channel.QueueBind(queue: qName, exchange: "direct_logs", routingKey: "warn", null);
                    channel.QueueBind(queue: qName, exchange: "direct_logs", routingKey: "info", null);
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
