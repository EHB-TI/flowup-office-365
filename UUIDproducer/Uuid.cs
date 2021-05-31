using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UUIDproducer
{
    class Uuid
    {
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
        public static void updateMockEvent()
        {
            string message =
              "<event><header>" +
              "<UUID>6f7962dc-b83e-11eb-a5e5-0242ac180003</UUID>" +
              "<sourceEntityId></sourceEntityId>" +
              "<organiserUUID></organiserUUID>" +
              "<organiserSourceEntityId></organiserSourceEntityId>" +
              "<method>UPDATE</method>" +
              "<origin>FrontEnd</origin>" +
              "<version>3</version>" +
              "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
              "</header>" +
              "<body>" +
              "<name>Office</name>" +
              "<startEvent>2021-05-24T09:00:00</startEvent>" +
              "<endEvent>2021-05-04T06:00:00</endEvent>" +
              "<description>Updated Event</description>" +
              "<location>Updated Location Office</location>" +
              "</body></event>";


            Task task = new Task(() => Producer.sendMessage(message, "event"));

            task.Start();

            Console.WriteLine("Starting up producer...");
            Consumer.getMessage();
        }
        
        //deleten van event met xml file
        public static void deleteMockEvent()
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
       
        //deleten van event met xml file
        public static void updateMockEventFromFrontEnd()
        {
            string message =
                          "<event><header>" +
                          "<UUID></UUID>" +  //this is going to be filled by UUID
                          "<sourceEntityId>2</sourceEntityId>" +
                          "<organiserUUID></organiserUUID>" + //this is going to be filled by UUID
                          "<organiserSourceEntityId>1</organiserSourceEntityId>" +
                          "<method>UPDATE</method>" +
                          "<origin>FrontEnd</origin>" +
                          "<version></version>" +
                          "<timestamp>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K") + "</timestamp>" +
                          "</header>" +
                          "<body>" +
                          "<name>Office</name>" +
                          "<startEvent>2021-05-24T09:00:00</startEvent>" +
                          "<endEvent>2021-05-04T06:00:00</endEvent>" +
                          "<description>Updated Event</description>" +
                          "<location>Updated Location Office</location>" +
                          "</body></event>";

            Task task = new Task(() => Producer.sendMessage(message, "UUID"));

            task.Start();
            Consumer.getMessage();

        }
//______________________________________________________________________________________________________________________________________________________
       //maken van MOCK event met xml file
        public static void createMockUser()
        {
            string message =
            "<event><header>" +
            "<UUID>becd4e5d-fba7-400a-b68f-f240f77b9f40</UUID>" +
            "<sourceEntityId></sourceEntityId>" +
            "<organiserUUID></organiserUUID>" +
            "<organiserSourceEntityId>5</organiserSourceEntityId>" +
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
        public static void updateUser()
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
        public static void deleteUser()
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

    }
}
