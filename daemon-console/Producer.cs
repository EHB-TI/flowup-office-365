using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace UUIDproducer
{
    class Producer
    {
        //versturen van message aan de consumer
        public static void sendMessage(string message, string severity)
        {
            //connectie
            var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //channel.ExchangeDeclare(exchange: "direct_logs",
                //                        type: "direct");

                XmlSchemaSet schema = new XmlSchemaSet();
                schema.Add("", "EventSchema.xsd");

                XDocument xml = XDocument.Parse(message, LoadOptions.SetLineInfo);


                //bool xmlValidation = true;

                //xml.Validate(schema, (sender, e) =>
                //{
                //    xmlValidation = false;
                //});

                //if (xmlValidation)
                //{
                    //Console.WriteLine("XML valid to send");

                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "direct_logs",
                                         routingKey: severity,
                                         basicProperties: null,
                                         body: body);


                    Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);

                    XDocument doc = XDocument.Parse(message);
                   
                //}
                //else
                //{
                //    Console.WriteLine("XML is niet geldig");
                //}

            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
        //versturen van message aan de consumer
        public static void sendMessageLogging(string message, string severity)
        {
            //connectie
            var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "",
                                         routingKey: severity,
                                         basicProperties: null,
                                         body: body);

                    Console.WriteLine(" [x] Sent logging '{0}':'{1}'", severity, message);

            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
