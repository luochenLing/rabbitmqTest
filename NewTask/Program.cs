using RabbitMQ.Client;
using System;
using System.Text;

namespace NewTask
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("worker will start...");

            using (var conn = RabbitmqHelper.GetConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.QueueDeclare(queue: "task_work_queue",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var properties = channel.CreateBasicProperties();
                    //持久化和非持久化
                    properties.DeliveryMode=2;
                    properties.Persistent = true;
                    for (int i = 0; i < 50; i++)
                    {
                        var message = "this is " + i + " message";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: "",
                                             routingKey: "task_work_queue",
                                             basicProperties: properties,
                                             body: body);
                    }
                    Console.WriteLine(" [x] Sent over");
                    Console.ReadLine();
                }
            }
        }
        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }

    }
}
