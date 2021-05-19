using MySql.Data.MySqlClient;
using RabbitMQ.Client;
using System;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

class EmitLogDirect
{
    public static void Main(string[] args)
    {

        //var factory = new ConnectionFactory() { HostName = "localhost" };
        var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "direct_logs",
                                    type: "direct");


            XmlDocument doc = new XmlDocument();
            doc.Load("CreateEvent.xml");

            XmlSchemaSet schema = new XmlSchemaSet();

            schema.Add("", "EventSchema.xsd");

            //XDocument xml = XDocument.Parse(xmlmessage, LoadOptions.SetLineInfo);
            XDocument xml = XDocument.Parse(doc.OuterXml);

            bool xmlValidation = true;

            xml.Validate(schema, (sender, e) =>
            {
                xmlValidation = false;
            });

            if (xmlValidation)
            {
                Console.WriteLine("XML is geldig");
            }
            else
            {
                Console.WriteLine("XML is ongeldig");
            }

            var severity = "event";
            string message = doc.InnerXml;

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "direct_logs",
                                 routingKey: severity,
                                 basicProperties: null,
                                 body: body);
            //Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
            Console.WriteLine(message);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

}