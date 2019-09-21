using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ReceiveLogs
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var conn = RabbitmqHelper.GetConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);
                    var queueName = channel.QueueDeclare("receiveLogs1", false, false, false, null).QueueName;
                    channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "", arguments: null);
                    //channel.BasicQos(0,1,false);
                    Console.WriteLine(" [*] Waiting for logs.");
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        //channel.BasicAck(ea.DeliveryTag,false);
                        Console.WriteLine(" [x] {0}", message);
                    };
                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
                //放在这里不会触发回调函数，无任何输出，但是rabbitmq的控制台消息会被消费，如果是手动确认的话控制台的消息不会被消费
                //Console.WriteLine(" Press [enter] to exit.");
                //Console.ReadLine();
            }
            //放在这里也是不会触发回调函数，无任何输出，但是rabbitmq的控制台消息会被消费，如果是手动确认的话控制台的消息不会被消费
            //Console.WriteLine(" Press [enter] to exit.");
            //Console.ReadLine();
            //这里写了任何代码好像都不会触发回调函数去消费消息
            //string a = "11";
            //a += "222";
        }
    }
}
