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
        int choice = -1;

        while (choice != 10)
        {
            Console.WriteLine("Please choose one of the following options:");
            Console.WriteLine("0. Create User");
            Console.WriteLine("1. Update User");
            Console.WriteLine("2. Delete User");
            Console.WriteLine("3. Create Event");
            Console.WriteLine("4. Update Event");
            Console.WriteLine("5. Delete Event");
            Console.WriteLine("6. wrong xml event");
            Console.WriteLine("7. wrong xml user");
            Console.WriteLine("8. subscribe");
            Console.WriteLine("9. unsubscribe");
            Console.WriteLine("10. Exit application");

            try
            {
                choice = int.Parse(Console.ReadLine());
            }
            catch (System.FormatException)
            {
                // Set to invalid value
                choice = -1;
            }

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

                string message = "Byeee, thanks fo visiting ;)";

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
                    <name>Nieuw event voor Ilias</name>
                    <startEvent>2021-06-03T12:00:00+00:00</startEvent>
                    <endEvent>2021-06-04T23:50:41+00:00</endEvent>
                    <description>Ilias event 17:56</description>
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
                            <version>3</version>
                            <timestamp>2021-05-29T22:51:32+02:00</timestamp>
                          </header>
                          <body>
                            <name>Update turo</name>
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

                string subscribe =
                    @"<eventSubscribe>
                      <header>
                        <UUID>67d6a40a-7a17-43f8-9204-3d85a5857cef</UUID>
                        <method>SUBSCRIBE</method>
                        <origin>FrontEnd</origin>
                        <version>1</version>
                        <sourceEntityId></sourceEntityId>
                        <timestamp>2021-05-31T02:36:19+02:00</timestamp>
                      </header>
                      <body>
                        <eventUUID>e6c2595e-9472-4a5f-a74c-ac90bcecd539</eventUUID>
                        <eventSourceEntityId>138</eventSourceEntityId>
                        <attendeeUUID>910470ce-1672-4476-b220-b1bbad889e90</attendeeUUID>
                        <attendeeSourceEntityId>1</attendeeSourceEntityId>
                      </body>
                    </eventSubscribe>";

                string unsubscribe =
                    @"<eventSubscribe>
                      <header>
                        <UUID>67d6a40a-7a17-43f8-9204-3d85a5857cef</UUID>
                        <method>UNSUBSCRIBE</method>
                        <origin>FrontEnd</origin>
                        <version/>
                        <sourceEntityId></sourceEntityId>
                        <timestamp>2021-05-31T02:37:07+02:00</timestamp>
                      </header>
                      <body>
                        <eventUUID>e6c2595e-9472-4a5f-a74c-ac90bcecd539</eventUUID>
                        <eventSourceEntityId>138</eventSourceEntityId>
                        <attendeeUUID>910470ce-1672-4476-b220-b1bbad889e90</attendeeUUID>
                        <attendeeSourceEntityId>1</attendeeSourceEntityId>
                      </body>
                    </eventSubscribe>";

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
                
                var severity = "";

                switch (choice)
                {
                    case 0:
                        message = createUserMessage;
                        severity = "user";
                        break;
                    case 1:
                        message = updateUserMessage;
                        severity = "user";
                        break;
                    case 2:
                        message = deleteUserMessage;
                        severity = "user";
                        break;
                    case 3:
                        message = createEvent;
                        severity = "event";
                        break;
                    case 4:
                        message = updateEvent;
                        severity = "event";
                        break;
                    case 5:
                        message = deleteEvent;
                        severity = "event";
                        break;
                    case 6:
                        message = "wrong xml event";
                        severity = "event";
                        break;
                    case 7:
                        message = "wrong xml user";
                        severity = "user";
                        break;
                    case 8:
                        message = subscribe;
                        severity = "event";
                        break;
                    case 9:
                        message = unsubscribe;
                        severity = "event";
                        break;
                    default:
                        Console.WriteLine("Byeee");
                        break;
                }

                


                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "direct_logs",
                                     routingKey: severity,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
            }


        }
            
    }

}