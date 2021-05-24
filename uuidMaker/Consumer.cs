
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace uuidMaker
{
    class Consumer
    {
        public static void getMessage()
        {
            
                var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "direct_logs",
                    type: "direct");
                    var queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName,
                    exchange: "direct_logs",
                    routingKey: "Office");
                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        Console.WriteLine(" [x] Received '{0}':'{1}'",
                        routingKey, message);
                        if (routingKey == "Office")
                        {
                            Console.WriteLine("create event");

                        }
                    };
                    channel.BasicConsume(queue: queueName,
                    autoAck: true,
                    consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }

            }
        }
    }
        

