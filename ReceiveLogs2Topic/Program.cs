using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ReceiveLogs2Topic
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var conn=RabbitmqHelper.GetConnection())
            {
                using (var channel=conn.CreateModel())
                {
                    channel.ExchangeDeclare("log_topic", ExchangeType.Topic);
                    var qName = channel.QueueDeclare().QueueName;
                    channel.QueueBind(qName, "log_topic", "*");
                    channel.QueueBind(qName, "log_topic", "test.*");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) => 
                    {
                        var body = ea.Body;
                        var msg = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"[reveive msg:]{msg}");
                    };
                    channel.BasicConsume(qName,true, consumer);
                    Console.WriteLine("press [enter] to exit");
                    Console.ReadLine();
                }
            }
        }
    }
}
