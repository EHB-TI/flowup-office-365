using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using RabbitMQ.Client;

namespace uuidMaker
{
    class Producer
    {
        //de kant dat de bericht stuurt
        public static void sendMessage(string severity,string message)
        {
            var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs",
                                        type: "direct");
                //xml hardcoded om te testen
                /*string message =

    "<event><header>" +
     "<UUID></UUID>" +
     "<sourceEntityId>aaa77</sourceEntityId>" +
     "<organiserUUID></organiserUUID>" +
     "<organiserSourceEntityId>1</organiserSourceEntityId >" +
     "<method>CREATE</method>" +
     "<origin>Office</origin>" +
     "<version>1</version>" +
     "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>"+
   "</header>" +
   "<body>" +
    " <name> Office</name>" +
     "<startEvent>2021-05-25T12:00:00</startEvent>" +
     "<endEvent>2021-05-27T02:00:00</endEvent>" +
     "<description>Description Office</description>" +
     "<location>Location Office</location>" +
   "</body>" +
 "</event>";*/
                

                XmlSchemaSet schema = new XmlSchemaSet();
                //event xsd validatie
                schema.Add("", "EventSchema.xsd");



                //xml file zal loaden
                XDocument xml = XDocument.Parse(message, LoadOptions.SetLineInfo);
                bool xmlValidation = true;

                xml.Validate(schema, (sender, e) =>
                {
                    xmlValidation = false;
                });

                if (xmlValidation)
                {
                    Console.WriteLine("XML is geldig");
                    //var severity = "UUID";
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "direct_logs",
                                         routingKey: severity,
                                         basicProperties: null,
                                         body: body);

                    
                    Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
                }
                else
                {
                    Console.WriteLine("XML is ongeldig");
                }
               
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
    