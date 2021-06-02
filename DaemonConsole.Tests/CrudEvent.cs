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
        [TestMethod]
        public void Subscribe_XML_Validation()
        {
            // Arrange
            TestingDaemon tDaemond = new TestingDaemon();
            docAlterSub.Load("AlterSubscribe.xml");

            // Act
            string myXmlSub = docAlterSub.InnerXml;
            bool r = tDaemond.getSubscribeValidation(myXmlSub);


            // Assert
            Assert.AreEqual(r, true, "Subscribe XML not valid");

        }
       
        [TestMethod]
        public void Error_XML_Validation()
        {
            // Arrange
            TestingDaemon tDaemond = new TestingDaemon();
            docAlterError.Load("AlterError.xml");

            // Act
            string myXmlError = docAlterError.InnerXml;
            bool r = tDaemond.getErrorValidation(myXmlError);


            // Assert
            Assert.AreEqual(r, false, "Error XML not valid");

        }

        [TestMethod]
        public void Log_XML_Validation()
        {
            // Arrange
            TestingDaemon tDaemond = new TestingDaemon();
           docAlterLog.Load("AlterLog.xml");

            // Act
            string myXmlLog = docAlterLog.InnerXml;
            bool r = tDaemond.getLogValidation(myXmlLog);


            // Assert
            Assert.AreEqual(r, false, "Log XML not valid");

        }
    }
}

