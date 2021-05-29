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

            var severity = "user";
            //string message = doc.InnerXml;
            string message = "<user><header>" +
              "<UUID>9ce7723f-1442-433e-a8cf-98af6d8cc197</UUID>" +
              "<method>CREATE</method>" +
              "<origin>AD</origin>" +
              "<version>1</version>" +
              "<sourceEntityId></sourceEntityId>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<firstname>mihriban</firstname>" +
              "<lastname>yelboga</lastname>" +
              "<email>mihriban.yelboga@student.ehb.be</email>" +
              "<birthday>1997-09-26</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>" +
              "</body></user>";
            string createUserMessageWrong = "<user><header>" +
              "<UUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</UUID>" +
              "<method>CREATE</method>" +
              "<origin>AD</origin>" +
              "<version>1</version>" +
              "<sourceEntityId>92c7ea28-b7c0-47a6-8a30-32cb02ce656e</sourceEntityId>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<firstname>Keanu</firstname>" +
              "<lastname>Piras</lastname>" +
              "<email>keanu.piras@student.dhs.be</email>" +
              "<birthday>2000-12-13</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>" +
              "</body></user>";
            string createUserMessage = "<user><header>" +
              "<UUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</UUID>" +
              "<method>CREATE</method>" +
              "<origin>AD</origin>" +
              "<version>1</version>" +
              "<sourceEntityId></sourceEntityId>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<firstname>Keanu</firstname>" +
              "<lastname>Piras</lastname>" +
              "<email>keanu.piras@student.dhs.be</email>" +
              "<birthday>2000-12-13</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>" +
              "</body></user>";
            string updateUserMessage = "<user><header>" +
              "<UUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</UUID>" +
              "<method>UPDATE</method>" +
              "<origin>AD</origin>" +
              "<version>2</version>" +
              "<sourceEntityId>92c7ea28-b7c0-47a6-8a30-32cb02ce656e</sourceEntityId>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<firstname>Keanu updated</firstname>" +
              "<lastname>Piras updated</lastname>" +
              "<email>updatedkeanu.piras@student.dhs.be</email>" +
              "<birthday>2000-12-13</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>" +
              "</body></user>";
            string deleteUserMessage = "<user><header>" +
              "<UUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</UUID>" +
              "<method>DELETE</method>" +
              "<origin>AD</origin>" +
              "<version>1</version>" +
              "<sourceEntityId></sourceEntityId>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<firstname>Keanu updated</firstname>" +
              "<lastname>Piras updated</lastname>" +
              "<email>keanu.piras@student.dhs.be</email>" +
              "<birthday>2000-12-13</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>" +
              "</body></user>";

            //var body = Encoding.UTF8.GetBytes(message);
            var body = Encoding.UTF8.GetBytes(deleteUserMessage);
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