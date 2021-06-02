using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace daemon_console
{
    public class TestingDaemon
    {
        public bool getEventValidation(string inputXML)
        {
            bool xmlValidation = true;
            //bool xmlValidationUser = true;
            //bool xmlValidationSubscribe = true;
            //bool xmlValidationError = true;

            XmlDocument xmlDoc = new XmlDocument();
            XDocument xml = new XDocument();

            xmlDoc.LoadXml(inputXML);
            xml = XDocument.Parse(xmlDoc.OuterXml);
            //xsd event validation
            XmlSchemaSet schema = new XmlSchemaSet();
            schema.Add("", "EventSchema.xsd");

            XmlSchemaSet schemaSubscribe = new XmlSchemaSet();
            schemaSubscribe.Add("", "SubscribeSchema.xsd");

            XmlSchemaSet schemaUser = new XmlSchemaSet();
            schemaUser.Add("", "UserSchema.xsd");

            XmlSchemaSet schemaError = new XmlSchemaSet();
            schemaError.Add("", "Errorxsd.xsd");

            xml.Validate(schema, (sender, e) =>
            {
                xmlValidation = false;
            });

            //string returnMessage = "";
            bool returnMessage = xmlValidation;

            return returnMessage;
        }
        public bool getUserValidation(string inputXML)
        {
            //bool xmlValidation = true;
            bool xmlValidationUser = true;
            //bool xmlValidationSubscribe = true;
            //bool xmlValidationError = true;

            XmlDocument xmlDoc = new XmlDocument();
            XDocument xml = new XDocument();

            xmlDoc.LoadXml(inputXML);
            xml = XDocument.Parse(xmlDoc.OuterXml);
            XmlSchemaSet schemaUser = new XmlSchemaSet();
            schemaUser.Add("", "UserSchema.xsd");

            xml.Validate(schemaUser, (sender, e) =>
            {
                xmlValidationUser = false;
            });

            //string returnMessage = "";
            //bool returnMessage = xmlValidationUser;

            //return returnMessage;
            return xmlValidationUser;
        }
    }
}
