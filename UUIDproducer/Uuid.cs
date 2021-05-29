using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UUIDproducer
{
    class Uuid
    {
       //maken van event met xml file
        public static void createEvent()
        {
            string message =
            "<event><header>" +
            "<UUID></UUID>" +
            "<sourceEntityId>miritest</sourceEntityId>" +
            "<organiserUUID></organiserUUID>" +
            "<organiserSourceEntityId>1</organiserSourceEntityId>" +
            "<method>CREATE</method>" +
            "<origin>Office</origin>" +
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

            Task task = new Task(() => Producer.sendMessage(message, Severitys.UUID.ToString()));

            task.Start();
            Consumer.getMessage();

        }
       //maken van MOCK event met xml file
        public static void createMockEvent()
        {
            string message =
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

            Task task = new Task(() => Producer.sendMessage(message, "event"));

            task.Start();
            Consumer.getMessage();

        }
        //updaten van event aan de hand van xml
        public static void updateEvent()
        {
            string message =
              "<event><header>" +
              "<UUID></UUID>" +
              "<sourceEntityId>aaa77</sourceEntityId>" +
              "<organiserUUID></organiserUUID>" +
              "<organiserSourceEntityId>1</organiserSourceEntityId>" +
              "<method>UPDATE</method>" +
              "<origin>Office</origin>" +
              "<version>1</version>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<name>Office</name>" +
              "<startEvent>2021-05-30T12:00:00</startEvent>" +
              "<endEvent>2021-05-30T02:00:00</endEvent>" +
              "<description>Description Office</description>" +
              "<location>Location Office</location>" +
              "</body></event>";


            Task task = new Task(() => Producer.sendMessage(message, Severitys.UUID.ToString()));

            task.Start();
            Consumer.getMessage();
        }
        //deleten van event met xml file
        public static void deleteEvent()
        {
            string message =
                          "<event><header>" +
                          "<UUID></UUID>" +
                          "<sourceEntityId>5</sourceEntityId>" +
                          "<organiserUUID></organiserUUID>" +
                          "<organiserSourceEntityId></organiserSourceEntityId>" +
                          "<method>DELETE</method>" +
                          "<origin>Office</origin>" +
                          "<version>1</version>" +
                          "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
                          "</header>" +
                          "<body>" +
                          "<name>Office</name>" +
                          "<startEvent>2021-05-25T12:00:00</startEvent>" +
                          "<endEvent>2021-05-27T02:00:00</endEvent>" +
                          "<description>Description Office</description>" +
                          "<location>Location Office</location>" +
                          "</body></event>";

            Task task = new Task(() => Producer.sendMessage(message, Severitys.UUID.ToString()));

            task.Start();
            Consumer.getMessage();

        }
        //________________________________________________________________________________________________________________________________________________
       //maken van MOCK event met xml file
        public static void createRealTestUser()
        {
            string message =
           "<user><header>" +
              "<UUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</UUID>" +
              "<method>CREATE</method>" +
              "<origin>AD</origin>" +
              "<version>1</version>" +
              "<sourceEntityId>92c7ea28-b7c0-47a6-8a30-32cb02ce656e</sourceEntityId>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<firstname>Keanu</firstname>" +
              "<lastname>Piras</lastname>" +
              "<email>keanu.piras@student.dhs.be</email>" +
              "<birthday>2000-12-13</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>"+
              "</body></user>";

            Task task = new Task(() => Producer.sendMessage(message, "user"));

            task.Start();
            Console.WriteLine("producer starting!");
            Consumer.getMessage();

        }
        public static void updateRealTestUser()
        {
            string message =
           "<user><header>" +
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
              "<email>keanu.piras@student.dhs.be updated</email>" +
              "<birthday>2000-12-13</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>"+
              "</body></user>";

            Task task = new Task(() => Producer.sendMessage(message, "user"));

            task.Start();
            Console.WriteLine("producer starting!");
            Consumer.getMessage();

        }
        public static void deleteRealTestUser()
        {
            string message =
           "<user><header>" +
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
              "<email>keanu.piras@student.dhs.be updated</email>" +
              "<birthday>2000-12-13</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>"+
              "</body></user>";

            Task task = new Task(() => Producer.sendMessage(message, "user"));

            task.Start();
            Console.WriteLine("producer starting!");
            Consumer.getMessage();

        }
        public static void createMockUser()
        {
            string message =
           "<user><header>" +
              "<UUID>9ce7723f-1442-433e-a8cf-98af6d8cc197</UUID>" +
              "<method>CREATE</method>" +
              "<origin>AD</origin>" +
              "<version>1</version>" +
              "<sourceEntityId></sourceEntityId>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<firstname>mihriban</firstname>" +
              "<lastname>yelboga</lastname>" +
              "<email>mihriban.yelboga@student.ehb.be</email>" +
              "<birthday>1997-09-26</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>"+
              "</body></user>";

            Task task = new Task(() => Producer.sendMessage(message, "user"));

            task.Start();
            Console.WriteLine("producer starting!");
            Consumer.getMessage();

        }
        //updaten van user aan de hand van xml
        public static void updateMockUser()
        {
            string message =
              "<user><header>" +
              "<UUID></UUID>" +
              "<method>UPDATE</method>" +
              "<origin>AD</origin>" +
              "<version>1</version>" +
              "<sourceEntityId></sourceEntityId>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<firstname>mihribanyel</firstname>" +
              "<lastname>yelbogaaa</lastname>" +
              "<email>mihriban.yelboga@student.ehb.be</email>" +
              "<birthday>1997-09-26</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>" +
              "</body></user>";



            Task task = new Task(() => Producer.sendMessage(message, "user"));

            task.Start();
            Console.WriteLine("Starting up producer...");

            Consumer.getMessage();
        }
        //deleten van user met xml file
        public static void deleteMockUser()
        {
            string message =
              "<user><header>" +
              "<UUID></UUID>" +
              "<method>DELETE</method>" +
              "<origin>AD</origin>" +
              "<version>1</version>" +
              "<sourceEntityId>7</sourceEntityId>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<firstname>test6666</firstname>" +
              "<lastname>yelboga</lastname>" +
              "<email>mihriban.yelboga@student.ehb.be</email>" +
              "<birthday>1997-09-26</birthday>" +
              "<role>student</role>" +
              "<study>Dig-X</study>" +
              "</body></user>";


            Task task = new Task(() => Producer.sendMessage(message,"user"));

            task.Start();
            Console.WriteLine("Starting up producer...");
            Consumer.getMessage();

        }

    }
}
