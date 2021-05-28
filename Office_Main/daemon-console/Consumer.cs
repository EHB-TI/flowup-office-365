using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using MySql.Data.MySqlClient;
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
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs",
                type: "direct");
                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName,
                exchange: "direct_logs",
                routingKey: "event");
                //channel.QueueBind(queue: queueName,
                //exchange: "direct_logs",
                //routingKey: "user");
                channel.QueueBind(queue: queueName,
                exchange: "direct_logs",
                routingKey: "Office");
                //channel.QueueBind(queue: queueName,
                //exchange: "direct_logs",
                //routingKey: "FrontEnd");
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
                    XmlSchemaSet schemaSubscribe = new XmlSchemaSet();
                    schema.Add("", "SubscribeSchema.xsd");
                    //XDocument xml = XDocument.Parse(message, LoadOptions.SetLineInfo);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(message);
                    XDocument xml = XDocument.Parse(xmlDoc.OuterXml);

                    bool xmlValidation = true;
                    bool xmlValidationSubscribe = true;

                    xml.Validate(schema, (sender, e) =>
                    {
                        xmlValidation = false;
                    });
                    xml.Validate(schemaSubscribe, (sender, e) =>
                    {
                        xmlValidationSubscribe = false;
                    });
                    //if (routingKey == "Office")
                    //{
                    // Console.WriteLine("create event");

                    //}
                    //XML from RabbitMQ

                    //Alter XML to change
                    XmlDocument docAlter = new XmlDocument();

                    if (xmlValidation)
                    {
                        
                        Console.WriteLine("XML is valid");

                        //XML head
                        XmlNode myMethodNode = xmlDoc.SelectSingleNode("//method");
                        XmlNode myOriginNode = xmlDoc.SelectSingleNode("//origin");
                        XmlNode myOrganiserSourceId = xmlDoc.SelectSingleNode("//organiserSourceEntityId");
                        XmlNode mySourceEntityId = xmlDoc.SelectSingleNode("//sourceEntityId");
                        //XML body
                        XmlNode myEventName = xmlDoc.SelectSingleNode("//name");
                        XmlNode myUserId = xmlDoc.SelectSingleNode("//organiserUUID");
                        XmlNode myStartEvent = xmlDoc.SelectSingleNode("//startEvent");
                        XmlNode myEndEvent = xmlDoc.SelectSingleNode("//endEvent");
                        XmlNode myDescription = xmlDoc.SelectSingleNode("//description");
                        XmlNode myLocation = xmlDoc.SelectSingleNode("//location");

                        //Create Event comes from FrontEnd and we pass it to UUID to compare
                        if (myOriginNode.InnerXml == "FrontEnd" && myMethodNode.InnerXml == "CREATE" && myOrganiserSourceId.InnerXml == "" && routingKey == "event")
                        {
                            Console.WriteLine("Got a message from " + myOriginNode.InnerXml);
                            Console.WriteLine("Updating origin \"" + myOriginNode.InnerXml + "\" of XML...");

                           

                            //XmlWriterSettings settings = new XmlWriterSettings();
                            //settings.Indent = true;
                            //XmlWriter writer = XmlWriter.Create("Alter.xml", settings);
                            //doc1.Save(writer);


                            docAlter.Load("Alter.xml");
                            docAlter = xmlDoc;

                            docAlter.SelectSingleNode("//event/header/origin").InnerText = "Office";
                            docAlter.Save("Alter.xml");


                            docAlter.Save(Console.Out);
                            //Console.WriteLine(docAlter.InnerXml);
                            //Console.WriteLine(docMessage.InnerXml);
                            //Console.WriteLine(docMessageConverted.InnerXml);


                            Task task = new Task(() => Producer.sendMessage(docAlter.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Origin changed to Office, sending now it to UUID...");

                        }
                        
                        //Create Event comes from UUID we use it and pass it again to UUID
                        else if (myOriginNode.InnerXml == "UUID" && myMethodNode.InnerXml == "CREATE" && myOrganiserSourceId.InnerXml != "" && routingKey == "Office")
                        {
                            


                            Console.WriteLine("Got a message from " + myOriginNode.InnerXml);
                            Console.WriteLine("Putting data in database and calendar");

                            string cs = @"server=10.3.56.8;userid=root;password=IUM_VDFt8ZQzc_sF;database=OfficeDB;Old Guids=True";
                            using var con = new MySqlConnection(cs);
                            con.Open();

                            var sql = "INSERT INTO Event(name, userId, startEvent, endEvent, description, location) VALUES(@name, @userId, @startEvent, @endEvent, @description, @location); SELECT @@IDENTITY";
                            using var cmd = new MySqlCommand(sql, con);




                            //Parse data to put into database
                            DateTime parsedDateStart;
                            DateTime parsedDateEnd;
                            parsedDateStart = DateTime.Parse(myStartEvent.InnerXml, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            parsedDateEnd = DateTime.Parse(myEndEvent.InnerXml, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                            cmd.Parameters.AddWithValue("@name", myEventName.InnerXml);
                            cmd.Parameters.AddWithValue("@userId", myOrganiserSourceId.InnerXml);
                            cmd.Parameters.AddWithValue("@startEvent", parsedDateStart);
                            cmd.Parameters.AddWithValue("@endEvent", parsedDateEnd);
                            cmd.Parameters.AddWithValue("@description", myDescription.InnerXml);
                            cmd.Parameters.AddWithValue("@location", myLocation.InnerXml);

                            int iNewRowIdentity = Convert.ToInt32(cmd.ExecuteScalar());
                            Console.WriteLine("Envet Id in database is: " + iNewRowIdentity);
                            Console.WriteLine("Event inserted in database");

                            docAlter.Load("Alter.xml");
                            docAlter = xmlDoc;

                            docAlter.SelectSingleNode("//event/header/origin").InnerText = "Office";
                            docAlter.SelectSingleNode("//event/header/sourceEntityId").InnerText = iNewRowIdentity.ToString();
                            docAlter.Save("Alter.xml");

                            docAlter.Save(Console.Out);

                            Task task = new Task(() => Producer.sendMessage(docAlter.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Sending message to UUID...");
                        }

                        //Update Event comes from FrontEnd and we pass it to UUID to compare
                        if (myOriginNode.InnerXml == "FrontEnd" && myMethodNode.InnerXml == "UPDATE" && myOrganiserSourceId.InnerXml == "" && routingKey == "event")
                        {
                            Console.WriteLine("Got a message from " + myOriginNode.InnerXml);
                            Console.WriteLine("Updating origin from \"" + myOriginNode.InnerXml + " to \"Office\"");


                            docAlter.Load("Alter.xml");
                            docAlter = xmlDoc;

                            docAlter.SelectSingleNode("//event/header/origin").InnerText = "Office";
                            docAlter.Save("Alter.xml");
                            docAlter.Save(Console.Out);


                            Task task = new Task(() => Producer.sendMessage(docAlter.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Origin changed to Office, sending now it to UUID...");

                        }

                        //Update Event comes from UUID, we update our side and tell the UUID
                        else if (myOriginNode.InnerXml == "UUID" && myMethodNode.InnerXml == "UPDATE" && mySourceEntityId.InnerXml != "" && routingKey == "Office")
                        {
                            Console.WriteLine("Got a message from " + myOriginNode.InnerXml);
                            //Console.WriteLine("Source id is: " + myDescription.InnerXml);
                            Console.WriteLine("Updating event data in database and calendar");

                            string cs = @"server=10.3.56.8;userid=root;password=IUM_VDFt8ZQzc_sF;database=OfficeDB;Old Guids=True";
                            using var con = new MySqlConnection(cs);
                            con.Open();

                            var sql = "UPDATE Event SET name = @name, startEvent = @startEvent, endEvent = @endEvent, description = @description, location = @location WHERE eventId = '" + mySourceEntityId.InnerXml + "'";
                            using var cmd = new MySqlCommand(sql, con);


                            //Parse data to put into database
                            DateTime parsedDateStart;
                            DateTime parsedDateEnd;
                            parsedDateStart = DateTime.Parse(myStartEvent.InnerXml, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                            parsedDateEnd = DateTime.Parse(myEndEvent.InnerXml, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                            cmd.Parameters.AddWithValue("@name", myEventName.InnerXml);
                            cmd.Parameters.AddWithValue("@startEvent", parsedDateStart);
                            cmd.Parameters.AddWithValue("@endEvent", parsedDateEnd);
                            cmd.Parameters.AddWithValue("@description", myDescription.InnerXml);
                            cmd.Parameters.AddWithValue("@location", myLocation.InnerXml);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Event inserted in database");

                            docAlter.Load("Alter.xml");
                            docAlter = xmlDoc;

                            docAlter.SelectSingleNode("//event/header/origin").InnerText = "Office";
                            docAlter.SelectSingleNode("//event/header/sourceEntityId").InnerText = mySourceEntityId.InnerXml;
                            docAlter.Save("Alter.xml");

                            docAlter.Save(Console.Out);

                            Task task = new Task(() => Producer.sendMessage(docAlter.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Sending update message to UUID (last step)...");
                        }
                        
                        //Delete Event comes from Front end and we pass it to UUID to compare
                        if (myOriginNode.InnerXml == "FrontEnd" && myMethodNode.InnerXml == "DELETE" && myOrganiserSourceId.InnerXml == "" && routingKey == "event")
                        {
                            Console.WriteLine("Got a message from " + myOriginNode.InnerXml);
                            Console.WriteLine("Updating origin from \"" + myOriginNode.InnerXml + " to \"Office\"");


                            docAlter.Load("Alter.xml");
                            docAlter = xmlDoc;

                            docAlter.SelectSingleNode("//event/header/origin").InnerText = "Office";
                            docAlter.Save("Alter.xml");
                            docAlter.Save(Console.Out);

                            Task task = new Task(() => Producer.sendMessage(docAlter.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Origin changed to Office, sending delete now it to UUID...");

                        }
                        
                        //Get delete info from UUID and delete it in db and in calendar
                        else if (myOriginNode.InnerXml == "UUID" && myMethodNode.InnerXml == "DELETE" && myOrganiserSourceId.InnerXml != "" && routingKey == "Office")
                        {
                            Console.WriteLine("Got a delete message from " + myOriginNode.InnerXml);
                            Console.WriteLine("The full message from the UUID is: " + xmlDoc.InnerXml);
                            //Console.WriteLine("Source id is: " + myDescription.InnerXml);
                            Console.WriteLine("Deleting event data in database and calendar");

                            string cs = @"server=10.3.56.8;userid=root;password=IUM_VDFt8ZQzc_sF;database=OfficeDB;Old Guids=True";
                            using var con = new MySqlConnection(cs);
                            con.Open();

                            var sql = "DELETE FROM Event WHERE eventId = '" + mySourceEntityId.InnerXml + "'";
                            using var cmd = new MySqlCommand(sql, con);

                            //cmd.Prepare();
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Event deleted in database");

                        }


                       

                        //XDocument xmlEvent = XDocument.Parse(message, LoadOptions.SetLineInfo);
                        //Console.WriteLine(xmlEvent);


                        //string xmlStuur = createEventXml(xmlEvent);
                        //Console.WriteLine(xmlStuur);
                    }
                    else if(xmlValidationSubscribe)
                    {
                        Console.WriteLine("Valid Subscribe XML");

                        //XML head
                        XmlNode myMethodNode = xmlDoc.SelectSingleNode("//method");
                        XmlNode myOriginNode = xmlDoc.SelectSingleNode("//origin");
                        XmlNode mySourceEntityId = xmlDoc.SelectSingleNode("//sourceEntityId");
                        XmlNode mySubVersion = xmlDoc.SelectSingleNode("//version");
                        //XML body
                        XmlNode myUUID = xmlDoc.SelectSingleNode("//eventUUID");
                        XmlNode myEventSourceEntityId = xmlDoc.SelectSingleNode("//eventSourceEntityId");
                        XmlNode myAttendeeUUID = xmlDoc.SelectSingleNode("//attendeeUUID");
                        XmlNode myAttendeeSourceEntityId = xmlDoc.SelectSingleNode("//attendeeSourceEntityId");

                        //Subscribe to Event
                        if (myOriginNode.InnerXml == "FrontEnd" && myMethodNode.InnerXml == "SUBSCRIBE")
                        {
                            Console.WriteLine("Subscriging to event, and putting it in database");
                        }
                        //Unubscribe to Event
                        if (myOriginNode.InnerXml == "FrontEnd" && myMethodNode.InnerXml == "UNSUBCRIBE")
                        {
                            Console.WriteLine("Unsusbscribing from event, and putting it in database");
                        }
                    }
                    //XML error from UUID
                    else
                    {
                        XmlNode myCodeNode = xmlDoc.SelectSingleNode("//code");
                        XmlNode myOriginNode = xmlDoc.SelectSingleNode("//origin");
                        XmlNode myobjectSourceId = xmlDoc.SelectSingleNode("//objectSourceId");
                        XmlNode myErrorMessage = xmlDoc.SelectSingleNode("//description");

                        Console.WriteLine("Error XML received");
                        Console.WriteLine("Message is: " + myErrorMessage.InnerXml);

                        schema = new XmlSchemaSet();
                        schema.Add("", "Errorxsd.xsd");
                        xml = XDocument.Parse(message, LoadOptions.SetLineInfo);
                        xmlValidation = true;

                        xml.Validate(schema, (sender, e) =>
                        {
                            xmlValidation = false;
                        });


                        /*XDocument xmlEvent = XDocument.Parse(message);
                        string error = "";
                        string code = "";

                        IEnumerable<XElement> xElements = xmlEvent.Descendants("description");
                        IEnumerable<XElement> xElements1 = xmlEvent.Descendants("code");
                        foreach (var element in xElements)
                        {
                            error = element.Value;

                        }
                        foreach (var element in xElements1)
                        {
                            code = element.Value;

                        }

                        Console.WriteLine(error);
                        Console.WriteLine(code);
                        Console.WriteLine("Code node" + myCodeNode.InnerXml);*/

                        //Event comes from UUID Master, we get a message back from UUID

                        switch (myCodeNode.InnerXml)
                        {
                            case "1000":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                            case "1001":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                            case "1002":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                            case "1003":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                            case "1004":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                            case "1005":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                                //DB Error
                            case "2000":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                                //Create
                            case "3000":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                            case "3001":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                            case "3002":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                                //Update
                            case "4000":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                            case "4001":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                                //Delete
                            case "5000":
                                Console.WriteLine("Code is: " + myCodeNode.InnerXml);
                                break;
                            default:
                                Console.WriteLine("Default case");
                                break;
                        }

                        

                    }

                };
                channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }

        }

        //private static string createEventXml(XDocument xmlEvent)
        //{
        //    //xml file attributen event
        //    string sourceEntityId = "";
        //    string organiserSourceEntityId = "";
        //    string method = "";
        //    string origin = "Office";
        //    string version = "";
        //    string timestamp = "";


        //    string name = "";
        //    string startEvent = "";
        //    string endEvent = "";
        //    string description = "";
        //    string location = "";



        //    IEnumerable<XElement> xElements = xmlEvent.Descendants("method");
        //    foreach (var element in xElements)
        //    {
        //        method = element.Value;
        //    }
        //    xElements = xmlEvent.Descendants("sourceEntityId");
        //    foreach (var element in xElements)
        //    {
        //        sourceEntityId = element.Value;
        //    }
        //    xElements = xmlEvent.Descendants("organiserSourceEntityId");
        //    foreach (var element in xElements)
        //    {
        //        organiserSourceEntityId = element.Value;
        //    }
        //    xElements = xmlEvent.Descendants("version");
        //    foreach (var element in xElements)
        //    {
        //        version = element.Value;
        //    }
        //    xElements = xmlEvent.Descendants("timestamp");
        //    foreach (var element in xElements)
        //    {
        //        timestamp = element.Value;
        //    }


        //    xElements = xmlEvent.Descendants("name");
        //    foreach (var element in xElements)
        //    {
        //        name = element.Value;
        //    }

        //    xElements = xmlEvent.Descendants("startEvent");
        //    foreach (var element in xElements)
        //    {
        //        startEvent = element.Value;
        //    }

        //    xElements = xmlEvent.Descendants("endEvent");
        //    foreach (var element in xElements)
        //    {
        //        endEvent = element.Value;
        //    }
        //    xElements = xmlEvent.Descendants("description");
        //    foreach (var element in xElements)
        //    {
        //        description = element.Value;
        //    }
        //    xElements = xmlEvent.Descendants("location");
        //    foreach (var element in xElements)
        //    {
        //        location = element.Value;
        //    }

        //    string message = "";
        //    xElements = xmlEvent.Descendants("UUID");
        //    xElements = xmlEvent.Descendants("organiserUUID");


        //    foreach (var el in xElements)
        //    {
        //        message =
        //    "<event><header>" +
        //    "<UUID>" + el.Value + "</UUID>" +
        //    "<sourceEntityId>" + sourceEntityId + "</sourceEntityId>" +
        //    "<organiserUUID>" + el.Value + "</organiserUUID>" +
        //    "<organiserSourceEntityId>" + organiserSourceEntityId + "</organiserSourceEntityId >" +
        //    "<method>" + method + "</method>" +
        //    "<origin>" + origin + "</origin>" +
        //    "<version>" + version + "</version>" +
        //    "<timestamp>" + timestamp + "</timestamp>" +
        //    "</header>" +
        //    "<body>" +
        //    "<name>" + name + "</name>" +
        //    "<startEvent>" + startEvent + "</startEvent>" +
        //    "<endEvent>" + endEvent + "</endEvent>" +
        //    "<description>" + description + "</description>" +
        //    "<location>" + location + "</location>" +
        //    "</body></event>";
        //    }


        //    return message;

        //}
    }
}
