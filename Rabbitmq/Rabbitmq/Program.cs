using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Xml;

namespace Rabbitmq
{
    class Program
    {
        static List<float> AvailableCPU = new List<float>();
        static List<float> AvailableRAM = new List<float>();

        protected static PerformanceCounter cpuCounter,ramCounter;


        static void Main(string[] args)
        {
            //Set CPU and RAM counter to sent w/ heartbeat
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");



            var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                while (true)
                {
                    //set message in xml
                    //string xmlmessage = "<heartbeat><header><code> 2000 </code >" +
                    //"<origin>Office</origin>" +
                    //$"<timestamp> {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K")} </timestamp ></header>" +
                    //$"<body><nameService> Website </nameService><CPUload>{getCurrentCpuUsage()}</CPUload>" +
                    //$"<RAMload> {getAvailableRAM()} </RAMload></body>" +
                    //"</heartbeat>";

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml("<?xml version='1.0' ?>" +
                    "<heartbeat><header><code> 2000 </code >" +
                    "<origin>Office</origin>" +
                    $"<timestamp> {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K")} </timestamp ></header>" +
                    $"<body><nameService> Website </nameService><CPUload>{getCurrentCpuUsage()}</CPUload>" +
                    $"<RAMload> {getAvailableRAM()} </RAMload></body>" +
                    "</heartbeat>");

                    string message = doc.InnerXml;
                    //string message = doc.InnerText; //shows only values
                    //string message = doc.DocumentElement.InnerText; //also shows values
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "hello", //tell to what qeue it has to go
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", message);

                    //stop the program for 1 second
                    Thread.Sleep(1000);

                }
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        static string getCurrentCpuUsage()
        {
            return cpuCounter.NextValue().ToString();
        }

        static string getAvailableRAM()
        {
            return ramCounter.NextValue().ToString();
        }
    }
}