using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;

class ReceiveLogsDirect
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        //var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "direct_logs",
                                    type: "direct");
            var queueName = channel.QueueDeclare().QueueName;

            //if (args.Length < 1)
            //{
            //    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
            //                            Environment.GetCommandLineArgs()[0]);
            //    Console.WriteLine(" Press [enter] to exit.");
            //    Console.ReadLine();
            //    Environment.ExitCode = 1;
            //    return;
            //}

            //foreach (var severity in args)
            //{
            //    channel.QueueBind(queue: queueName,
            //                      exchange: "direct_logs",
            //                      routingKey: severity);
            //}


            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add("", "EventSchema.xsd");

            //Used to do this do this when validation is good of wrong
            bool xmlValidation = true;

            channel.QueueBind(queue: queueName,
                                exchange: "direct_logs",
                                routingKey: "info");
            channel.QueueBind(queue: queueName,
                                exchange: "direct_logs",
                                routingKey: "createevnt");
            channel.QueueBind(queue: queueName,
                                exchange: "direct_logs",
                                routingKey: "deleteevent");

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;
                //Console.WriteLine(" [x] Received '{0}':'{1}'",
                //                  routingKey, message);
                Console.WriteLine(message);

                //Convert string message to xmlDoc and after to XDoc to Validate
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(message);
                XDocument xml = XDocument.Parse(xmlDoc.OuterXml);

                xml.Validate(schema, (sender, e) =>
                {
                    xmlValidation = false;
                });


                if (xmlValidation)
                {
                    Console.WriteLine("XML is geldig");
                    XmlNode myMethodNode = xmlDoc.SelectSingleNode("//method");
                    XmlNode myOriginNode = xmlDoc.SelectSingleNode("//origin");
                    XmlNode myNameNode = xmlDoc.SelectSingleNode("//name");
                    XmlNode myLocationNode = xmlDoc.SelectSingleNode("//location");
                    Console.WriteLine("Method is: " + myMethodNode.InnerXml);
                    Console.WriteLine("Origin is: " + myOriginNode.InnerXml);
                    Console.WriteLine("Name is: " + myNameNode.InnerXml);
                    Console.WriteLine("Location is: " + myLocationNode.InnerXml);
                }
                else
                {
                    Console.WriteLine("XML is ongeldig");
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