using System.Threading;
using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace recevExchange
{
    class Program
    {
        static void Main(string[] args)
        {
            int random = new Random().Next(1, 10000);
            Console.WriteLine("Hello World!");
            IConnectionFactory confacotry = new ConnectionFactory
            {
                HostName = "192.168.53.4",
                Port = 5672,
                UserName = "admin",
                Password = "admin"
            };
            using (IConnection con = confacotry.CreateConnection())
            using (IModel channel = con.CreateModel())
            {
                string queueName = string.Empty;
                string exchangeName = "exchange2";
                // if (args.Length > 0)
                // {
                //     exchangeName = args[0];
                // }
                // else
                // {
                //     exchangeName = "exchange1";
                // }
                channel.ExchangeDeclare(exchange: exchangeName, type: "direct");
                queueName = exchangeName + "_" + random;
                //声明一个队列
                channel.QueueDeclare(
                    queue: queueName,
                    durable: false,//是否缓存
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                //匹配多个路由
                foreach (var item in args)
                {
                    //将队列与交换机进行绑定
                    channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: item);

                }

                channel.BasicQos(0, 1, false);
                //创建消费者对象
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    Thread.Sleep(1000);
                    byte[] message = ea.Body;
                    Console.WriteLine("接收到信息为:" + Encoding.UTF8.GetString(message));
                    channel.BasicAck(ea.DeliveryTag, true);
                };
                //消费者开启监听
                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                Console.ReadLine();
            }
        }
    }
}
