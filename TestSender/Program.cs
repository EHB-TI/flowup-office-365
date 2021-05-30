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


            //XmlDocument doc = new XmlDocument();
            //doc.Load("CreateEvent.xml");
            //XmlDocument doc = new XmlDocument();
            //doc.Load("DeleteEvent.xml");

            //XmlSchemaSet schema = new XmlSchemaSet();

            //schema.Add("", "EventSchema.xsd");

            ////XDocument xml = XDocument.Parse(xmlmessage, LoadOptions.SetLineInfo);
            //XDocument xml = XDocument.Parse(doc.OuterXml);

            //bool xmlValidation = true;

            string message = "no";

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

            string createEvent = @"<event>
                  <header>
                    <UUID>4f2bca51-a3ba-4228-8c29-c5891cef2b6a</UUID>
                    <sourceEntityId></sourceEntityId>
                    <organiserUUID>910470ce-1672-4476-b220-b1bbad889e90</organiserUUID>
                    <organiserSourceEntityId></organiserSourceEntityId>
                    <method>CREATE</method>
                    <origin>FrontEnd</origin>
                    <version>1</version>
                    <timestamp>2021-05-29T22:50:11+02:00</timestamp>
                  </header>
                  <body>
                    <name>Test</name>
                    <startEvent>2021-05-30T12:00:00+00:00</startEvent>
                    <endEvent>2021-05-30T23:50:41+00:00</endEvent>
                    <description>Nog een event</description>
                    <location>Online</location>
                  </body>
                </event>";

            string updateEvent = @"<event>
                          <header>
                            <UUID>4f2bca51-a3ba-4228-8c29-c5891cef2b6a</UUID>
                            <sourceEntityId></sourceEntityId>
                            <organiserUUID/>
                            <organiserSourceEntityId></organiserSourceEntityId>
                            <method>UPDATE</method>
                            <origin>FrontEnd</origin>
                            <version>2</version>
                            <timestamp>2021-05-29T22:51:32+02:00</timestamp>
                          </header>
                          <body>
                            <name>Test</name>
                            <startEvent>2021-05-30T12:00:00+00:00</startEvent>
                            <endEvent>2021-05-30T23:50:41+00:00</endEvent>
                            <description>Nog een event bewerkt</description>
                            <location>Online</location>
                          </body>
                        </event>";

            string deleteEvent = @"<event>
                      <header>
                        <UUID>4f2bca51-a3ba-4228-8c29-c5891cef2b6a</UUID>
                        <sourceEntityId>
                        </sourceEntityId>
                        <organiserUUID>910470ce-1672-4476-b220-b1bbad889e90</organiserUUID>
                        <organiserSourceEntityId>
                        </organiserSourceEntityId>
                        <method>DELETE</method>
                        <origin>FrontEnd</origin>
                        <version />
                        <timestamp>2021-05-29T22:52:09+02:00</timestamp>
                      </header>
                      <body>
                        <name>Test</name>
                        <startEvent>2021-05-30T12:00:00+00:00</startEvent>
                        <endEvent>2021-05-30T23:50:41+00:00</endEvent>
                        <description>Nog een event bewerkt</description>
                        <location>Online</location>
                      </body>
                        </event>";

            //xml.Validate(schema, (sender, e) =>
            //{
            //    xmlValidation = false;
            //});

            //if (xmlValidation)
            //{
            //    Console.WriteLine("XML is geldig");
            //}
            //else
            //{
            //    Console.WriteLine("XML is ongeldig");

            //}

            //message = doc.InnerXml;
            //message = createUserMessage;
            //message = updateUserMessage;
            //message = deleteUserMessage;
            message = createEvent;
            //message = updateEvent;
            //message = deleteEvent;

            var severity = "event";
            //var severity = "user";


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