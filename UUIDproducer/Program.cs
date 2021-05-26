using System;
using System.Threading.Tasks;

namespace UUIDproducer
{
    class Program
    {
        static void Main(string[] args)
        {


            Uuid.deleteEvent();


            /*
            Task task = new Task(() => Producer.send());
            task.Start();
            Consumer.getMessage();

            */
        }
    }
}
