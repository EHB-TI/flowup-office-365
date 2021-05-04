using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Management;

namespace Rabbitmq
{
    class Program
    {
        static List<float> AvailableCPU = new List<float>();
        static List<float> AvailableRAM = new List<float>();
        protected static double totalRam = 0;

        protected static PerformanceCounter cpuCounter, ramCounter;


        static void Main(string[] args)
        {
            //Set CPU and RAM counter to sent w/ heartbeat
            //cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            //ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available KBytes");

            ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
            ManagementObjectCollection results = searcher.Get();

            foreach (ManagementObject result in results)
            {
                totalRam = Convert.ToDouble(result["TotalVisibleMemorySize"]);

            }



            var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //channel.QueueDeclare(queue: "hello",
                channel.QueueDeclare(queue: "heartbeat",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);


                while (true)
                {

                    ////Update the XML
                    //XmlDocument doc2 = (XmlDocument)doc.Clone();
                    XmlDocument doc = new XmlDocument();
                    doc.Load("XMLHeartBeat.xml");

                    //XmlNode root = doc.DocumentElement;
                    XmlNode myDateNode = doc.SelectSingleNode("//timestamp");
                    XmlNode myCPUNode = doc.SelectSingleNode("//CPUload");
                    XmlNode myRAMNode = doc.SelectSingleNode("//RAMload");
                    //XmlNode myOriginNode = doc.SelectSingleNode("//origin");
                    //myOriginNode.InnerText = "No"; //this should make an Error
                    //myCPUNode.InnerText = "Arturo"; //this should make an Error
                    myDateNode.InnerText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K");
                    myCPUNode.InnerText = getCpuUsage();
                    myRAMNode.InnerText = getRamUsage();

                    doc.Save("XMLHeartBeat.xml");

                    //if (IsFileReady("XMLHeartBeat.xml"))
                    //{
                    //    Console.WriteLine("Ready");
                    //}




                    //Console.WriteLine(myDateNode.InnerXml); //for testing

                    //XmlDocument doc = new XmlDocument();
                    //doc.LoadXml("<?xml version='1.0' ?>" +
                    //"<heartbeat><header><code> 2000 </code >" +
                    //"<origin>Office</origin>" +
                    //$"<timestamp> {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss%K")} </timestamp ></header>" +
                    //$"<body><nameService> Website </nameService><CPUload>{getCurrentCpuUsage()}</CPUload>" +
                    //$"<RAMload> {getAvailableRAM()} </RAMload></body>" +
                    //"</heartbeat>");

                    //doc.LoadXml("XMLHeartBeat.xml");


                    //Validate agains XSD
                    //try
                    //{
                    //    XmlReaderSettings settings = new XmlReaderSettings();
                    //    settings.Schemas.Add("", "XMLHeartBeat.xsd");
                    //    settings.ValidationType = ValidationType.Schema;

                    //    XmlReader reader = XmlReader.Create("XMLHeartBeat.xml", settings);
                    //    XmlDocument document = new XmlDocument();
                    //    document.Load(reader);

                    //    ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);

                    //    // the document will now fail to successfully validate
                    //    document.Validate(eventHandler);


                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //    break;
                    //}

                    string message = doc.InnerXml;
                    //string message = doc.InnerText; //shows only values
                    //string message = doc.DocumentElement.InnerText; //also shows values
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: "heartbeat", //tell to what qeue it has to go
                                        //routingKey: "hello", //tell to what qeue it has to go
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

        //static string getCurrentCpuUsage()
        //{
        //    return cpuCounter.NextValue().ToString();
        //}

        //static string getAvailableRAM()
        //{
        //    return ramCounter.NextValue().ToString();
        //}

        static string getCpuUsage()
        {
            string cpu = cpuCounter.NextValue().ToString();
            string text = cpu.Replace(',', '.');

            return text;
        }

        static string getRamUsage()
        {
            double usageRam = ramCounter.NextValue();

            double avRam = (usageRam / totalRam * 100);

            return avRam.ToString().Replace(',', '.');
        }

        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Console.WriteLine("Error: {0}", e.Message);
                    Console.WriteLine("what?");
                    break;
                case XmlSeverityType.Warning:
                    Console.WriteLine("Warning {0}", e.Message);
                    Console.WriteLine("woot");
                    break;
            }
        }

        public static bool IsFileReady(string filename)

        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public static void WaitForFile(string filename)
        //{
        //    //This will lock the execution until the file is ready
        //    //TODO: Add some logic to make it async and cancelable
        //    while (!IsFileReady(filename)) { Console.WriteLine("Not ready"); }
        //}
    }
}