using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using daemon_console;
using Microsoft.Graph;
using MySql.Data.MySqlClient;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;



namespace UUIDproducer
{
    class Consumer
    {

        public static void getMessage()
        {
            //try
            //{
            //    Program.RunAsync().GetAwaiter().GetResult();
            //}
            //catch (Exception ex)
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(ex.Message);
            //    Console.ResetColor();
            //}

            string cs = "";

            try
            {
                cs = System.IO.File.ReadAllText("cs.txt");
            

            //Console.WriteLine("cs string is: " + cs);

            var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs",
                type: "direct");

                //var queueName = channel.QueueDeclare().QueueName;
                var queueName = "officeQueue";
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);


                channel.QueueBind(queue: queueName,
                exchange: "direct_logs",
                routingKey: "event");
                channel.QueueBind(queue: queueName,
                exchange: "direct_logs",
                routingKey: "user");
                channel.QueueBind(queue: queueName,
                exchange: "direct_logs",
                routingKey: "Office");
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
                    XmlSchemaSet schemaUser = new XmlSchemaSet();
                    schemaUser.Add("", "UserSchema.xsd");
                    //XDocument xml = XDocument.Parse(message, LoadOptions.SetLineInfo);
                    XmlDocument xmlDoc = new XmlDocument();
                    try
                    {
                        xmlDoc.LoadXml(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Weird message came in: " + message);
                    }
                    XDocument xml = XDocument.Parse(xmlDoc.OuterXml);

                    //fixed
                    bool xmlValidation = true;
                    bool xmlValidationUser = true;
                    bool xmlValidationSubscribe = true;

                    xml.Validate(schema, (sender, e) =>
                    {
                        xmlValidation = false;
                    });
                    xml.Validate(schemaUser, (sender, e) =>
                    {
                        xmlValidationUser = false;
                    });
                    xml.Validate(schemaSubscribe, (sender, e) =>
                    {
                        xmlValidationSubscribe = false;
                    });
                  

                    //Alter XML to change
                    XmlDocument docAlter = new XmlDocument();

                    if (xmlValidation)
                    {
                        
                        Console.WriteLine("XML is valid");

                        //XML head
                        XmlNode myMethodNode = xmlDoc.SelectSingleNode("//method");
                        XmlNode myOriginNode = xmlDoc.SelectSingleNode("//origin");
                        XmlNode myUserId = xmlDoc.SelectSingleNode("//organiserUUID");
                        XmlNode myOrganiserSourceId = xmlDoc.SelectSingleNode("//organiserSourceEntityId");
                        XmlNode mySourceEntityId = xmlDoc.SelectSingleNode("//sourceEntityId");
                        //XML body
                        XmlNode myEventName = xmlDoc.SelectSingleNode("//name");
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
                        
                        //Create Event comes from UUID we use it and pass it again to UUID (Last step create)
                        else if (myOriginNode.InnerXml == "UUID" && myMethodNode.InnerXml == "CREATE" && myOrganiserSourceId.InnerXml != "" && routingKey == "Office")
                        {

                            Console.WriteLine(myStartEvent.InnerXml);
                            List<Attendee> attendees = new List<Attendee>();
                            try
                            {
                                string startTimeEvent = myStartEvent.InnerXml.Substring(0, (myStartEvent.InnerXml.Length - 6));
                                string endTimeEvent = myEndEvent.InnerXml.Substring(0, (myEndEvent.InnerXml.Length - 6));
                                Program.RunAsync("create",myEventName.InnerXml,myDescription.InnerXml, startTimeEvent, endTimeEvent,
                                    myLocation.InnerXml, attendees,true,"null").GetAwaiter().GetResult();
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(ex.Message);
                                Console.ResetColor();
                            }

                            Console.WriteLine("Got a message from " + myOriginNode.InnerXml);
                            Console.WriteLine("Putting data in database and calendar");

                            using var con = new MySqlConnection(cs);
                            con.Open();


                            //var sql = "INSERT INTO Event(name, userId, graphResponse, startEvent, endEvent, description, location) VALUES(@name, @userId, @graphResponse, @startEvent, @endEvent, @description, @location); SELECT @@IDENTITY";
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

                            //cmd.Parameters.AddWithValue("@graphResponse", stringjsonData);

                            //int iNewRowIdentity = 0; 
                            //try
                            //{
                            //    iNewRowIdentity = Convert.ToInt32(cmd.ExecuteScalar());
                            //    int iNewRowIdentity = Convert.ToInt32(cmd.ExecuteScalar());

                            //}catch(MySqlException e)
                            //{
                            //    Console.WriteLine("Message: " + e.Message);
                            //}
                            int iNewRowIdentity = Convert.ToInt32(cmd.ExecuteScalar());
                            Console.WriteLine("Event Id in database is: " + iNewRowIdentity);


                           

                            Console.WriteLine("Event inserted in database");

                            docAlter.Load("Alter.xml");
                            docAlter = xmlDoc;

                            docAlter.SelectSingleNode("//event/header/origin").InnerText = "Office";
                            docAlter.SelectSingleNode("//event/header/sourceEntityId").InnerText = iNewRowIdentity.ToString();
                            docAlter.Save("Alter.xml");

                            docAlter.Save(Console.Out);

                            Task task = new Task(() => Producer.sendMessage(docAlter.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Sending message to UUID (Last step)...");
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

                            using var con = new MySqlConnection(cs);
                            con.Open();

                            var sql = "DELETE FROM Event WHERE eventId = '" + mySourceEntityId.InnerXml + "'";
                            using var cmd = new MySqlCommand(sql, con);

                            //cmd.Prepare();
                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Event deleted in database");

                        }
                    }
                    else if (xmlValidationUser)
                    {
                        XmlDocument docAlterUser = new XmlDocument();
                        Console.WriteLine("XML user is valid");

                        //XML HEAD
                        XmlNode myMethodNodeUser = xmlDoc.SelectSingleNode("//method");
                        XmlNode myOriginNodeUser = xmlDoc.SelectSingleNode("//origin");
                        XmlNode mySourceIdUser = xmlDoc.SelectSingleNode("//sourceEntityId");
                        XmlNode myUserVersion = xmlDoc.SelectSingleNode("//version");

                        //XML BODY
                        XmlNode myFirstName = xmlDoc.SelectSingleNode("//firstname");
                        XmlNode myLastName = xmlDoc.SelectSingleNode("//lastname");
                        XmlNode myEmail = xmlDoc.SelectSingleNode("//email");
                        XmlNode myBirthday = xmlDoc.SelectSingleNode("//birthday");
                        XmlNode myRole = xmlDoc.SelectSingleNode("//role");
                        XmlNode myStudy = xmlDoc.SelectSingleNode("//study");


                        //CREATE User, message from AD
                        //if (myOriginNodeUser.InnerXml == "AD" && myMethodNodeUser.InnerXml == "CREATE" && mySourceIdUser.InnerXml == "" && routingKey == "user")
                        if (myOriginNodeUser.InnerXml == "AD" && myMethodNodeUser.InnerXml == "CREATE" && routingKey == "user")
                        {
                            Console.WriteLine("Got a message from " + myOriginNodeUser.InnerXml);
                            Console.WriteLine("Updating origin \"" + myOriginNodeUser.InnerXml + "\" of XML...");



                            docAlterUser.Load("AlterUser.xml");
                            docAlterUser = xmlDoc;

                            docAlterUser.SelectSingleNode("//user/header/origin").InnerText = "Office";
                            docAlterUser.SelectSingleNode("//user/header/sourceEntityId").InnerText = "";
                            docAlterUser.Save("AlterUser.xml");


                            docAlterUser.Save(Console.Out);


                            Task task = new Task(() => Producer.sendMessage(docAlterUser.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Origin changed to Office, sending now it to UUID...");
                        }
                        //Message from UUID CREATE user now
                        //else if (myOriginNodeUser.InnerXml == "UUID" && myMethodNodeUser.InnerXml == "CREATE" && mySourceIdUser.InnerXml == "" && routingKey == "Office")
                        else if (myOriginNodeUser.InnerXml == "UUID" && myMethodNodeUser.InnerXml == "CREATE" && routingKey == "Office")
                        {

                            Console.WriteLine("Got a message from " + myOriginNodeUser.InnerXml);
                            Console.WriteLine("Creating user, and putting it in database");


                            //connection to the database
                            using var con = new MySqlConnection(cs);
                            con.Open();

                            var sql = "INSERT INTO User(firstname,lastname,email,birthday,role,study) VALUES(@firstname, @lastname, @email, @birthday,@role,@study); SELECT @@IDENTITY";
                            using var cmd = new MySqlCommand(sql, con);

                            //Parse data to put into database
                            DateTime parsedBirthday;

                            parsedBirthday = DateTime.Parse(myBirthday.InnerXml, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                            cmd.Parameters.AddWithValue("@firstname", myFirstName.InnerXml);
                            cmd.Parameters.AddWithValue("@lastname", myLastName.InnerXml);
                            cmd.Parameters.AddWithValue("@email", myEmail.InnerXml);
                            cmd.Parameters.AddWithValue("@birthday", parsedBirthday);
                            cmd.Parameters.AddWithValue("@role", myRole.InnerXml);
                            cmd.Parameters.AddWithValue("@study", myStudy.InnerXml);


                            int iNewRowIdentity = Convert.ToInt32(cmd.ExecuteScalar());
                            Console.WriteLine("User Id in database is: " + iNewRowIdentity);
                            Console.WriteLine("User inserted in database");


                            docAlterUser.Load("AlterUser.xml");
                            docAlterUser = xmlDoc;

                            docAlterUser.SelectSingleNode("//user/header/origin").InnerText = "Office";
                            docAlterUser.SelectSingleNode("//user/header/sourceEntityId").InnerText = iNewRowIdentity.ToString();
                            docAlterUser.Save("AlterUser.xml");


                            docAlter.Save(Console.Out);


                            Task task = new Task(() => Producer.sendMessage(docAlterUser.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Sending creating user message to UUID...");
                        }
                        //UPDATE user comes from AD and we pass it to UUID to compare
                        //if (myOriginNodeUser.InnerXml == "AD" && myMethodNodeUser.InnerXml == "UPDATE" && mySourceIdUser.InnerXml == "" && routingKey == "user")
                        if (myOriginNodeUser.InnerXml == "AD" && myMethodNodeUser.InnerXml == "UPDATE" && routingKey == "user")
                        {

                            Console.WriteLine("Got a message from " + myOriginNodeUser.InnerXml);
                            Console.WriteLine("updating user, and putting it in database");

                            docAlterUser.Load("AlterUser.xml");
                            docAlterUser = xmlDoc;

                            docAlterUser.SelectSingleNode("//user/header/origin").InnerText = "Office";
                            docAlterUser.SelectSingleNode("//user/header/sourceEntityId").InnerText = "";
                            docAlterUser.Save("AlterUser.xml");
                            docAlterUser.Save(Console.Out);



                            Task task = new Task(() => Producer.sendMessage(docAlterUser.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Origin changed to Office, sending now it to UUID...");
                        }
                        //update user comes from UUID we update our side and tell the UUID
                        //else if (myOriginNodeUser.InnerXml == "UUID" && myMethodNodeUser.InnerXml == "UPDATE" && mySourceIdUser.InnerXml == "" && routingKey == "Office")
                        else if (myOriginNodeUser.InnerXml == "UUID" && myMethodNodeUser.InnerXml == "UPDATE" && routingKey == "Office")
                        {
                            Console.WriteLine("Got a message from " + myOriginNodeUser.InnerXml);
                            //Console.WriteLine("Source id is: " + myDescription.InnerXml);
                            Console.WriteLine("Updating user data in database and calendar");


                            try
                            {


                                using var con = new MySqlConnection(cs);
                                con.Open();

                                var sql = "UPDATE User SET firstname = @firstname, lastname  = @lastname, email = @email, birthday = @birthday, role = @role, study = @study WHERE userId = '" + mySourceIdUser.InnerXml + "'";
                                using var cmd = new MySqlCommand(sql, con);

                                //Parse data to put into database
                                DateTime parsedBirthday;

                                parsedBirthday = DateTime.Parse(myBirthday.InnerXml, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                                //cmd.Parameters.AddWithValue("@userId", mySourceIdUser.InnerXml);
                                cmd.Parameters.AddWithValue("@firstname", myFirstName.InnerXml);
                                cmd.Parameters.AddWithValue("@lastname", myLastName.InnerXml);
                                cmd.Parameters.AddWithValue("@email", myEmail.InnerXml);
                                cmd.Parameters.AddWithValue("@birthday", parsedBirthday);
                                cmd.Parameters.AddWithValue("@role", myRole.InnerXml);
                                cmd.Parameters.AddWithValue("@study", myStudy.InnerXml);
                                cmd.Prepare();
                                cmd.ExecuteNonQuery();

                                Console.WriteLine("User inserted in database");
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine("User not yet in our database");
                                Console.WriteLine("Exception is: " + e.Message);
                            }

                            //int iNewRowIdentity = Convert.ToInt32(cmd.ExecuteScalar());
                            //Console.WriteLine("User Id in database is: " + iNewRowIdentity);



                            docAlterUser.Load("AlterUser.xml");
                            docAlterUser = xmlDoc;

                            docAlterUser.SelectSingleNode("//user/header/origin").InnerText = "Office";
                            //docAlter.SelectSingleNode("//user/header/userId").InnerText = mySourceIdUser.InnerXml;
                            docAlterUser.Save("AlterUser.xml");


                            docAlterUser.Save(Console.Out);


                            Task task = new Task(() => Producer.sendMessage(docAlterUser.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Sending update user message to UUID...");


                        }
                        //Delete user comes from AD we update our side and tell the UUID
                        //if (myOriginNodeUser.InnerXml == "UUID" && myMethodNodeUser.InnerXml == "DELETE" && mySourceIdUser.InnerXml == "" && routingKey == "Office")
                        if (myOriginNodeUser.InnerXml == "AD" && myMethodNodeUser.InnerXml == "DELETE" && routingKey == "user")
                        {
                            Console.WriteLine("Got a message from " + myOriginNodeUser.InnerXml);
                            Console.WriteLine("updating user, and putting it in database");

                            docAlterUser.Load("AlterUser.xml");
                            docAlterUser = xmlDoc;

                            docAlterUser.SelectSingleNode("//user/header/origin").InnerText = "Office";
                            docAlterUser.SelectSingleNode("//user/header/sourceEntityId").InnerText = "";
                            docAlterUser.Save("AlterUser.xml");
                            docAlterUser.Save(Console.Out);



                            Task task = new Task(() => Producer.sendMessage(docAlterUser.InnerXml, "UUID"));
                            task.Start();

                            Console.WriteLine("Origin changed to Office, sending now it to UUID...");
                        }
                        //else if(myOriginNodeUser.InnerXml == "UUID" && myMethodNodeUser.InnerXml == "DELETE" && mySourceIdUser.InnerXml != "" && routingKey == "Office")
                        else if (myOriginNodeUser.InnerXml == "UUID" && myMethodNodeUser.InnerXml == "DELETE" && routingKey == "Office")
                        {
                            Console.WriteLine("Got a delete message from " + mySourceIdUser.InnerXml);
                            Console.WriteLine("The full message from the UUID is: " + xmlDoc.InnerXml);
                            //Console.WriteLine("Source id is: " + myDescription.InnerXml);
                            Console.WriteLine("Deleting user data in database and calendar");

                            try
                            {

                                using var con = new MySqlConnection(cs);
                                con.Open();
                                var sql = "DELETE FROM User WHERE userId= '" + mySourceIdUser.InnerXml + "'";

                                using var cmd = new MySqlCommand(sql, con);

                                cmd.ExecuteNonQuery();
                                Console.WriteLine("User deleted from database");
                            }
                            catch (SqlException e)
                            {
                                Console.WriteLine("Exception message: " + e.Message);
                            }

                        }
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
                        //header
                        XmlNode myCodeNode = xmlDoc.SelectSingleNode("//code");
                        XmlNode myOriginNode = xmlDoc.SelectSingleNode("//origin");
                        XmlNode myTimestamp = xmlDoc.SelectSingleNode("//timestamp");
                        //body
                        XmlNode myobjectUUID = xmlDoc.SelectSingleNode("//objectUUID");
                        XmlNode myobjectSourceId = xmlDoc.SelectSingleNode("//objectSourceId");
                        XmlNode myObjectOrigin = xmlDoc.SelectSingleNode("//objectOrigin");
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
            catch (IOException e)
            {
                Console.WriteLine("cs file could not be read:");
                Console.WriteLine(e.Message);
            }

        }
    }
}
