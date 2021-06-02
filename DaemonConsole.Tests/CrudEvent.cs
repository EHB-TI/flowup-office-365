using daemon_console;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using UUIDproducer;

namespace DaemonConsole.Tests
{
    [TestClass]
    public class CrudEvent
    {
        //Alter XML to change
        XmlDocument docAlter = new XmlDocument();
        XmlDocument docAlterUser = new XmlDocument();
        XmlDocument docAlterSub = new XmlDocument();
        XmlDocument docAlterError = new XmlDocument();
        XmlDocument docAlterLog = new XmlDocument();

            

        [TestMethod]
        public void Event_XML_Validation()
        {
            // Arrange
            TestingDaemon tDaemond = new TestingDaemon();

            docAlter.Load("Alter.xml");
            docAlterUser.Load("AlterUser.xml");
            docAlterLog.Load("AlterLog.xml");
            docAlterSub.Load("AlterSubscribe.xml");
            docAlterError.Load("AlterError.xml");

            string myXmlEvent = docAlter.InnerXml;

            // Act
            bool r = tDaemond.getEventValidation(myXmlEvent);


            // Assert
            Assert.AreEqual(r, true, "Event XML not valid");

        }

        [TestMethod]
        public void User_XML_Validation()
        {
            // Arrange
            TestingDaemon tDaemond = new TestingDaemon();
            docAlterUser.Load("AlterUser.xml");

            // Act
            string myXmlUser = docAlterUser.InnerXml;
            bool r = tDaemond.getUserValidation(myXmlUser);


            // Assert
            Assert.AreEqual(r, true, "User XML not valid");

        }
    }
}

