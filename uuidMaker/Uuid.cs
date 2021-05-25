using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace uuidMaker
{
    class Uuid
    {
        public static void createEvent()
        {
            string message=
            "<event><header>" +
            "<UUID></UUID>" +
            "<sourceEntityId>aaa77</sourceEntityId>" +
            "<organiserUUID></organiserUUID>" +
            "<organiserSourceEntityId>1</organiserSourceEntityId >" +
            "<method>CREATE</method>" +
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
            "</body>" +
            "</event>";

            Task task = new Task(() => Producer.sendMessage(message, severitys.UUID.ToString()));

            task.Start();
            Consumer.getMessage();

        }

        public static void updateEvent()
        {
            string message =
              "<event><header>" +
              "<UUID></UUID>" +
              "<sourceEntityId>aaa77</sourceEntityId>" +
              "<organiserUUID></organiserUUID>" +
              "<organiserSourceEntityId>1</organiserSourceEntityId >" +
              "<method>CREATE</method>" +
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
              "</body>" +
              "</event>";

            Task task = new Task(() => Producer.sendMessage(message, severitys.UUID.ToString()));

            task.Start();
            Consumer.getMessage();
        }

        public static void deleteEvent()
        {
            string message =
                          "<event><header>" +
                          "<UUID></UUID>" +
                          "<sourceEntityId>aaa77</sourceEntityId>" +
                          "<organiserUUID></organiserUUID>" +
                          "<organiserSourceEntityId>1</organiserSourceEntityId >" +
                          "<method>CREATE</method>" +
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
                          "</body>" +
                          "</event>";

            Task task = new Task(() => Producer.sendMessage(message, severitys.UUID.ToString()));

            task.Start();
            Consumer.getMessage();

        }
    }
}
