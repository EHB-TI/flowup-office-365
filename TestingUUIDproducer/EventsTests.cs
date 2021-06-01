using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RabbitMQ.Client;
using Xunit;
using daemon_console.GraphCrud;
using daemon_console;
using System.Xml;
using System;

namespace TestingUUIDproducer
{
    
    public class CreateEventTests
    {
      


        string testEvent=
            "<event><header>" +
            "<UUID>52b2f72f-8ed7-4d34-8b3e-c6f54c0791c8</UUID>" +
            "<sourceEntityId></sourceEntityId>" +
            "<organiserUUID>910470ce-1672-4476-b220-b1bbad889e90</organiserUUID>" +
            "<organiserSourceEntityId></organiserSourceEntityId>" +
            "<method>CREATE</method>" +
            "<origin>FrontEnd</origin>" +
            "<version>1</version>" +
            "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
            "</header>" +
            "<body>" +
            "<name>EventNameOffice</name>" +
            "<startEvent>2021-06-12T12:00:00</startEvent>" +
            "<endEvent>2021-06-13T02:00:00</endEvent>" +
            "<description>testing</description>" +
            "<location>Location Office</location>" +
            "</body></event>";

        [Fact]
        public void testCreateEvent(string XmlDoc)
        {
            XmlDocument docAlter = new XmlDocument();
            XmlDocument xmlDoc = new XmlDocument();
            docAlter.Load(testEvent);
            docAlter = xmlDoc;

            docAlter.SelectSingleNode("//event/header/origin").InnerText = "Office";
            docAlter.Save("Alter.xml");


            docAlter.Save(Console.Out);

            XmlNode myMethodNode = xmlDoc.SelectSingleNode("//method");
            XmlNode myOriginNode = xmlDoc.SelectSingleNode("//origin");
            XmlNode myUserId = xmlDoc.SelectSingleNode("//organiserUUID");
            XmlNode myOrganiserSourceId = xmlDoc.SelectSingleNode("//organiserSourceEntityId");
            XmlNode mySourceEntityId = xmlDoc.SelectSingleNode("//sourceEntityId");
           

            XmlNode myEventName = xmlDoc.SelectSingleNode("//name");
            XmlNode myStartEvent = xmlDoc.SelectSingleNode("//startEvent");
            XmlNode myEndEvent = xmlDoc.SelectSingleNode("//endEvent");
            XmlNode myDescription = xmlDoc.SelectSingleNode("//description");
            XmlNode myLocation = xmlDoc.SelectSingleNode("//location");
            //arrange


            //act


            //assert


        }
    }
}
