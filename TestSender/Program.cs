using MySql.Data.MySqlClient;
using RabbitMQ.Client;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

class EmitLogDirect
{
    public static void Main(string[] args)
    {
        int choice = -1;

        while (choice != 100)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" 0. Create User");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" 1. Update User");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" 2. Delete User\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" 3. Create Event");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(" 4. Update Event");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" 5. Delete Event\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" 6. wrong xml event");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" 7. wrong xml user\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" 8. subscribe");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" 9. unsubscribe\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" 10. error3000 to logging\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" Enter to EXIT");
            Console.ResetColor();

            try
            {
                choice = int.Parse(Console.ReadLine());
            }
            catch (System.FormatException)
            {
                // Set to invalid value
                choice = 100;
            }

            var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //channel.ExchangeDeclare(exchange: "direct_logs",
                //                        type: "direct");


                string message = "Byeee, thanks fo visiting ;)";
                string myMessage = "";

                //string createUserMessage = "<user><header>" +
                //  "<UUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</UUID>" +
                //  "<method>CREATE</method>" +
                //  "<origin>AD</origin>" +
                //  "<version>1</version>" +
                //  "<sourceEntityId></sourceEntityId>" +
                //  "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
                //  "</header>" +
                //  "<body>" +
                //  "<firstname>Turo test app</firstname>" +
                //  "<lastname>DK</lastname>" +
                //  "<email>turo.mock@student.dhs.be</email>" +
                //  "<birthday>2000-12-13</birthday>" +
                //  "<role>student</role>" +
                //  "<study>Dig-X</study>" +
                //  "</body></user>";

                string createUserMessage = @"<user><header>
                  <UUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</UUID>
                  <method>CREATE</method>
                  <origin>AD</origin>
                  <version>1</version>
                  <sourceEntityId></sourceEntityId>
                  <timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + @"</timestamp>
                  </header>
                  <body>
                  <firstname>Turo test app</firstname>
                  <lastname>DK</lastname>
                  <email>turo.mock@student.dhs.be</email>
                  <birthday>2000-12-13</birthday>
                  <role>student</role>
                  <study>Dig-X</study>
                  </body></user>";

                string updateUserMessage = "<user><header>" +
                  "<UUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</UUID>" +
                  "<method>UPDATE</method>" +
                  "<origin>AD</origin>" +
                  "<version>2</version>" +
                  "<sourceEntityId>92c7ea28-b7c0-47a6-8a30-32cb02ce656e</sourceEntityId>" +
                  "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
                  "</header>" +
                  "<body>" +
                  "<firstname>Keanu updated From Turo test app</firstname>" +
                  "<lastname>Piras updated From Turo test app</lastname>" +
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
                  "<firstname>Turo test app</firstname>" +
                  "<lastname>Turo test app</lastname>" +
                  "<email>turo.mock@student.dhs.be</email>" +
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
                    <timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + @"</timestamp>
                  </header>
                  <body>
                    <name>Nieuw event voor Ilias</name>
                    <startEvent>2021-06-03T12:00:00+00:00</startEvent>
                    <endEvent>2021-06-04T23:50:41+00:00</endEvent>
                    <description>Ilias event 17:56 From Turo test app</description>
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
                            <timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + @"</timestamp>
                          </header>
                          <body>
                            <name>Update turo</name>
                            <startEvent>2021-05-30T12:00:00+00:00</startEvent>
                            <endEvent>2021-05-30T23:50:41+00:00</endEvent>
                            <description>Nog een event bewerktFrom Turo test app</description>
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
                        <timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + @"</timestamp>
                      </header>
                      <body>
                        <name>Test</name>
                        <startEvent>2021-05-30T12:00:00+00:00</startEvent>
                        <endEvent>2021-05-30T23:50:41+00:00</endEvent>
                        <description>Nog een event bewerktFrom Turo test app</description>
                        <location>Online</location>
                      </body>
                        </event>";

                //string subscribe =
                //    @"<eventSubscribe>
                //      <header>
                //        <UUID>67d6a40a-7a17-43f8-9204-3d85a5857cef</UUID>
                //        <method>SUBSCRIBE</method>
                //        <origin>FrontEnd</origin>
                //        <version>1</version>
                //        <sourceEntityId></sourceEntityId>
                //        <timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + @"</timestamp>
                //      </header>
                //      <body>
                //        <eventUUID>e6c2595e-9472-4a5f-a74c-ac90bcecd539</eventUUID>
                //        <eventSourceEntityId>138</eventSourceEntityId>
                //        <attendeeUUID>910470ce-1672-4476-b220-b1bbad889e90</attendeeUUID>
                //        <attendeeSourceEntityId>1</attendeeSourceEntityId>
                //      </body>
                //    </eventSubscribe>";

                //string unsubscribe =
                //    @"<eventSubscribe>
                //      <header>
                //        <UUID>67d6a40a-7a17-43f8-9204-3d85a5857cef</UUID>
                //        <method>UNSUBSCRIBE</method>
                //        <origin>FrontEnd</origin>
                //        <version/>
                //        <sourceEntityId></sourceEntityId>
                //        <timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + @"</timestamp>
                //      </header>
                //      <body>
                //        <eventUUID>e6c2595e-9472-4a5f-a74c-ac90bcecd539</eventUUID>
                //        <eventSourceEntityId>138</eventSourceEntityId>
                //        <attendeeUUID>910470ce-1672-4476-b220-b1bbad889e90</attendeeUUID>
                //        <attendeeSourceEntityId>1</attendeeSourceEntityId>
                //      </body>
                //    </eventSubscribe>";

                string subscribe =
                    @"<eventSubscribe>
                      <header>
                        <UUID>67d6a40a-7a17-43f8-9204-3d85a5857cef</UUID>
                        <method>SUBSCRIBE</method>
                        <origin>FrontEnd</origin>
                        <version>1</version>
                        <sourceEntityId></sourceEntityId>
                        <timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + @"</timestamp>
                      </header>
                      <body>
                        <eventUUID>4f2bca51-a3ba-4228-8c29-c5891cef2b6a</eventUUID>
                        <eventSourceEntityId>138</eventSourceEntityId>
                        <attendeeUUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</attendeeUUID>
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
                        <timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + @"</timestamp>
                      </header>
                      <body>
                        <eventUUID>4f2bca51-a3ba-4228-8c29-c5891cef2b6a</eventUUID>
                        <eventSourceEntityId>138</eventSourceEntityId>
                        <attendeeUUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</attendeeUUID>
                        <attendeeSourceEntityId>1</attendeeSourceEntityId>
                      </body>
                    </eventSubscribe>";

                string error300 =
                    @"<error>
                          <header>
                            <code>3000</code>
                            <origin>Office</origin>
                            <timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + @"</timestamp>
                          </header>
                          <body>
                            <objectUUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</objectUUID>
                            <objectSourceId/>
                            <objectOrigin>Office</objectOrigin>
                            <description>From Turo test app Object has already been added to the UUID master with source 'Office' and sourceEntityId '' and/or UUID 'becd4e5d-fba7-400a-b68f-f240f77b9f40'</description>
                          </body>
                        </error>";

                bool xmlValidation = true;
                bool xmlValidationUser = true;
                bool xmlValidationSubscribe = true;
                bool xmlValidationError = true;

                XmlSchemaSet schema = new XmlSchemaSet();
                schema.Add("", "EventSchema.xsd");

                XmlSchemaSet schemaSubscribe = new XmlSchemaSet();
                schemaSubscribe.Add("", "SubscribeSchema.xsd");

                XmlSchemaSet schemaUser = new XmlSchemaSet();
                schemaUser.Add("", "UserSchema.xsd");

                XmlSchemaSet schemaError = new XmlSchemaSet();
                schemaError.Add("", "Errorxsd.xsd");

                XmlDocument xmlDoc = new XmlDocument();
                XDocument xml = new XDocument();

             


                var severity = "";

                switch (choice)
                {
                    case 0:
                        message = createUserMessage;
                        myMessage = createUserMessage;
                        severity = "user";
                        break;
                    case 1:
                        message = updateUserMessage;
                        myMessage = updateUserMessage;
                        severity = "user";
                        break;
                    case 2:
                        message = deleteUserMessage;
                        myMessage = deleteUserMessage;
                        severity = "user";
                        break;
                    case 3:
                        message = createEvent;
                        myMessage = createEvent;
                        severity = "event";
                        break;
                    case 4:
                        message = updateEvent;
                        myMessage = updateEvent;
                        severity = "event";
                        break;
                    case 5:
                        message = deleteEvent;
                        myMessage = deleteEvent;
                        severity = "event";
                        break;
                    case 6:
                        message = "wrong xml event";
                        myMessage = "wrong xml event";
                        severity = "event";
                        break;
                    case 7:
                        message = "wrong xml user";
                        myMessage = "wrong xml user";
                        severity = "user";
                        break;
                    case 8:
                        message = subscribe;
                        myMessage = subscribe;
                        severity = "event";
                        break;
                    case 9:
                        message = unsubscribe;
                        myMessage = unsubscribe;
                        severity = "event";
                        break;
                    case 10:
                        message = error300;
                        myMessage = error300;
                        severity = "logging";
                        break;
                    default:
                        Console.WriteLine("Byeee");
                        break;
                }

                //xmlDoc.LoadXml(myMessage);
                //xml = XDocument.Parse(xmlDoc.OuterXml);
                //xml.Validate(schema, (sender, e) =>
                //{
                //    xmlValidation = false;
                //});
                //xml.Validate(schemaUser, (sender, e) =>
                //{
                //    xmlValidationUser = false;
                //});
                //xml.Validate(schemaSubscribe, (sender, e) =>
                //{
                //    xmlValidationSubscribe = false;
                //});
                //xml.Validate(schemaError, (sender, e) =>
                //{
                //    xmlValidationError = false;
                //});

                //Console.WriteLine("Event: " + xmlValidation);
                //Console.WriteLine("User: " + xmlValidationUser);
                //Console.WriteLine("Sub: " + xmlValidationSubscribe);
                //Console.WriteLine("Error: " + xmlValidationError);


                Console.ForegroundColor = ConsoleColor.White;
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "direct_logs",
                                     routingKey: severity,
                                     basicProperties: null,
                                     body: body);
                //channel.BasicPublish(exchange: "",
                //                     routingKey: severity,
                //                     basicProperties: null,
                //                     body: body);
                Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
            }


        }
            
    }

}