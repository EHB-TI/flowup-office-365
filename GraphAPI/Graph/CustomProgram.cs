
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace GraphTutorial
{
    class CustomProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".NET Core Graph Tutorial\n");

            Process.Start("explorer.exe", "https://microsoft.com/devicelogin");

            // <InitializationSnippet>
            var appConfig = LoadAppSettings();

            if (appConfig == null)
            {
                Console.WriteLine("Missing or invalid appsettings.json...exiting");
                return;
            }

            var appId = appConfig["appId"];
            var scopesString = appConfig["scopes"];
            var scopes = scopesString.Split(';');

            // Initialize the auth provider with values from appsettings.json
            var authProvider = new DeviceCodeAuthProvider(appId, scopes);

            // Request a token to sign in the user
            var accessToken = authProvider.GetAccessToken().Result;
            // </InitializationSnippet>

            // <GetUserSnippet>
            // Initialize Graph client
            GraphHelper.Initialize(authProvider);

            // Get signed in user
            var user = GraphHelper.GetMeAsync().Result;
            Console.WriteLine($"Welcome {user.DisplayName}!\n");
            // </GetUserSnippet>



            var factory = new ConnectionFactory() { HostName = "localhost" };
            //var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs",
                                        type: "direct");
                var queueName = channel.QueueDeclare().QueueName;


                XmlSchemaSet schema = new XmlSchemaSet();
                schema.Add("", "EventSchema.xsd");

                //Used to do this do this when validation is good of wrong
                bool xmlValidation = true;


                channel.QueueBind(queue: queueName,
                                    exchange: "direct_logs",
                                    routingKey: "event");

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                      routingKey, message);
                    //Console.WriteLine(message);

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
                        XmlNode myGueststNode = xmlDoc.SelectSingleNode("//guests");
                        Console.WriteLine("Method is: " + myMethodNode.InnerXml);
                        Console.WriteLine("Origin is: " + myOriginNode.InnerXml);
                        Console.WriteLine("Name is: " + myNameNode.InnerXml);
                        Console.WriteLine("Location is: " + myLocationNode.InnerXml);


                        var attendeeList = new List<string>();
                        //attendeeList.Add("arthur.de.keersmaeker@student.ehb.be");
                        attendeeList = myGueststNode.InnerXml.Split(',').ToList();


                        string userTimeZone = user.MailboxSettings.TimeZone;
                        string subject = xmlDoc.SelectSingleNode("//name").InnerXml;
                        //var start = DateTime.Parse("12/07/2021 12:00:00");
                        //var end = DateTime.Parse("12/07/2021 13:00:00");
                        DateTime start = XmlConvert.ToDateTime(xmlDoc.SelectSingleNode("//startEvent").InnerXml);
                        DateTime end = XmlConvert.ToDateTime(xmlDoc.SelectSingleNode("//endEvent").InnerXml);
                        string body1 = xmlDoc.SelectSingleNode("//description").InnerXml;

                        GraphHelper.CreateEvent(
                                userTimeZone,
                                subject,
                                start,
                                end,
                                attendeeList,
                                body1).Wait();

                        Console.WriteLine("userTimeZone: " + userTimeZone);
                        Console.WriteLine("subject: " + subject);
                        Console.WriteLine("start: " + start);
                        Console.WriteLine("end: " + end);
                        Console.WriteLine("attendeeList: " + attendeeList);
                        Console.WriteLine("body: " + body);
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



            //var attendeeList = new List<string>();
            //attendeeList.Add("arthur.de.keersmaeker@student.ehb.be");

            //string userTimeZone = user.MailboxSettings.TimeZone;
            ////string timezone = "Romance Standard Time";
            //string subject = "CodeSubject";
            //var start = DateTime.Parse("12/07/2021 12:00:00");
            //var end = DateTime.Parse("12/07/2021 13:00:00");
            //string body = "codeBody";

            //GraphHelper.CreateEvent(
            //        userTimeZone,
            //        subject,
            //        start,
            //        end,
            //        attendeeList,
            //        body).Wait();
        }

        

        // <LoadAppSettingsSnippet>
        static IConfigurationRoot LoadAppSettings()
        {
            var appConfig = new ConfigurationBuilder()
                .AddUserSecrets<CustomProgram>()
                .Build();

            // Check for required settings
            if (string.IsNullOrEmpty(appConfig["appId"]) ||
                string.IsNullOrEmpty(appConfig["scopes"]))
            {
                return null;
            }

            return appConfig;
        }
        // </LoadAppSettingsSnippet>
    }
}

