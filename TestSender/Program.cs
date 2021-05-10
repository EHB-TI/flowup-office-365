using MySql.Data.MySqlClient;
using RabbitMQ.Client;
using System;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

class EmitLogDirect
{
    public static void Main(string[] args)
    {
        string cs = @"server=10.3.56.10;userid=Office;password=Office2021;database=FlowUpDB";

        using var con = new MySqlConnection(cs);
        con.Open();

        //var stm = "SELECT VERSION()";
        //var cmd = new MySqlCommand(stm, con);

        //var version = cmd.ExecuteScalar().ToString();
        //Console.WriteLine($"MySQL version: {version}");

        string sql = "SELECT * FROM VoorbeeldTabel";
        //string testUUID = "5698cd59-3acc-4f15-9ce2-83545cbfe0ba";
        //string sql = "SELECT * FROM VoorbeeldTabel WHERE uuid = '" + testUUID + "'";
        using var cmd = new MySqlCommand(sql, con);
        MySqlDataReader rdr = cmd.ExecuteReader();
        //try
        //{
        //    MySqlDataReader rdr = cmd.ExecuteReader();
        //}
        //catch(MySqlException ex)
        //{
        //    Console.WriteLine(ex.Message);
        //}

        //DataTable dt = new DataTable();
        //MySqlDataAdapter da = new MySqlDataAdapter(cmd);
        //da.Fill(dt);
        //int i = Convert.ToInt32(dt.Rows.Count.ToString());

        //if(i == 0)
        //{
        //    Console.WriteLine("Failed");
        //}

        while (rdr.Read())
        {
            //Console.WriteLine("{0} {1} {2}", rdr.GetInt32(0), rdr.GetString(1),
            //        rdr.GetInt32(2));
            Console.WriteLine("{0,-2} {1} {2,-6} {3,-6} {4,-1} {5}", rdr.GetInt32(0), rdr.GetValue(1), rdr.GetString(2), rdr.GetString(3), rdr.GetInt32(4), rdr.GetString(5));
        }






        //var factory = new ConnectionFactory() { HostName = "localhost" };
        //var factory = new ConnectionFactory() { HostName = "10.3.56.6" };
        //using (var connection = factory.CreateConnection())
        //using (var channel = connection.CreateModel())
        //{
        //    channel.ExchangeDeclare(exchange: "direct_logs",
        //                            type: "direct");

        //    //var severity = (args.Length > 0) ? args[0] : "info";
        //    //var message = (args.Length > 1)
        //    //              ? string.Join(" ", args.Skip(1).ToArray())
        //    //              : "Hello World!";

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load("CreateEvent.xml");

        //    XmlSchemaSet schema = new XmlSchemaSet();

        //    schema.Add("", "EventSchema.xsd");

        //    //XDocument xml = XDocument.Parse(xmlmessage, LoadOptions.SetLineInfo);
        //    XDocument xml = XDocument.Parse(doc.OuterXml);

        //    bool xmlValidation = true;

        //    xml.Validate(schema, (sender, e) =>
        //    {
        //        xmlValidation = false;
        //    });

        //    if (xmlValidation)
        //    {
        //        Console.WriteLine("XML is geldig");
        //    }
        //    else
        //    {
        //        Console.WriteLine("XML is ongeldig");
        //    }

        //    var severity = "event";
        //    string message = doc.InnerXml;

        //    var body = Encoding.UTF8.GetBytes(message);
        //    channel.BasicPublish(exchange: "direct_logs",
        //                         routingKey: severity,
        //                         basicProperties: null,
        //                         body: body);
        //    //Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
        //    Console.WriteLine(message);
        //}

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

}