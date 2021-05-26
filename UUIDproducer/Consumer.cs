using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace UUIDproducer
{
    class Consumer
    {
        //de kant dat de bericht gaat krijgen
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
                routingKey: "UUID");
                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);


                    //xsd event validation
                    XmlSchemaSet schema = new XmlSchemaSet();
                    schema.Add("", "EventSchema.xsd");
                    XDocument xml = XDocument.Parse(message, LoadOptions.SetLineInfo);

                    bool xmlValidation = true;

                    xml.Validate(schema, (sender, e) =>
                    {
                        xmlValidation = false;
                    });
                    //if (routingKey == "Office")
                    //{
                    // Console.WriteLine("create event");

                    //}
                    if (xmlValidation)
                    {
                        Console.WriteLine("XML is geldig");
                        XDocument xmlEvent = XDocument.Parse(message, LoadOptions.SetLineInfo);

                        //string xmlStuur = createEventXml(xmlEvent);

                        Console.WriteLine(xmlEvent);

                       //sturen van bericht
                    }
                    //error afhandeling
                    else
                    {
                        schema = new XmlSchemaSet();
                        schema.Add("", "Errorxsd.xsd");
                        Console.WriteLine("XML is ongeldig");
                        xml = XDocument.Parse(message, LoadOptions.SetLineInfo);
                        xmlValidation = true;

                        xml.Validate(schema, (sender, e) =>
                        {
                            xmlValidation = false;
                        });


                        XDocument xmlEvent = XDocument.Parse(message);
                        string error = "";

                        /*IEnumerable<XElement> xElements = xmlEvent.Descendants("description");
                        foreach (var element in xElements)
                        {
                            error = element.Value;

                        }*/

                        Console.WriteLine(error);

                    }

                };
                channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }

        }

        /*private static string createEventXml(XDocument xmlEvent)
        {
            //xml file attributen event
            string sourceEntityId = "";
            string organiserSourceEntityId = "";
            string method = "";
            string origin = "Office";
            string version = "";
            string timestamp = "";


            string name = "";
            string startEvent = "";
            string endEvent = "";
            string description = "";
            string location = "";



            IEnumerable<XElement> xElements = xmlEvent.Descendants("method");
            foreach (var element in xElements)
            {
                method = element.Value;
            }
            xElements = xmlEvent.Descendants("sourceEntityId");
            foreach (var element in xElements)
            {
                sourceEntityId = element.Value;
            }
            xElements = xmlEvent.Descendants("organiserSourceEntityId");
            foreach (var element in xElements)
            {
                organiserSourceEntityId = element.Value;
            }
            xElements = xmlEvent.Descendants("version");
            foreach (var element in xElements)
            {
                version = element.Value;
            }
            xElements = xmlEvent.Descendants("timestamp");
            foreach (var element in xElements)
            {
                timestamp = element.Value;
            }


            xElements = xmlEvent.Descendants("name");
            foreach (var element in xElements)
            {
                name = element.Value;
            }

            xElements = xmlEvent.Descendants("startEvent");
            foreach (var element in xElements)
            {
                startEvent = element.Value;
            }

            xElements = xmlEvent.Descendants("endEvent");
            foreach (var element in xElements)
            {
                endEvent = element.Value;
            }
            xElements = xmlEvent.Descendants("description");
            foreach (var element in xElements)
            {
                description = element.Value;
            }
            xElements = xmlEvent.Descendants("location");
            foreach (var element in xElements)
            {
                location = element.Value;
            }

            string message = "";
            xElements = xmlEvent.Descendants("UUID");
            xElements = xmlEvent.Descendants("organiserUUID");


            foreach (var el in xElements)
            {
                message =
            "<event><header>" +
            "<UUID>" + el.Value + "</UUID>" +
            "<sourceEntityId>" + sourceEntityId + "</sourceEntityId>" +
            "<organiserUUID>" + el.Value + "</organiserUUID>" +
            "<organiserSourceEntityId>" + organiserSourceEntityId + "</organiserSourceEntityId >" +
            "<method>" + method + "</method>" +
            "<origin>" + origin + "</origin>" +
            "<version>" + version + "</version>" +
            "<timestamp>" + timestamp + "</timestamp>" +
            "</header>" +
            "<body>" +
            "<name>" + name + "</name>" +
            "<startEvent>" + startEvent + "</startEvent>" +
            "<endEvent>" + endEvent + "</endEvent>" +
            "<description>" + description + "</description>" +
            "<location>" + location + "</location>" +
            "</body></event>";
            }


            return message;

        }
    }*/
}
