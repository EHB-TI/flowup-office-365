using System;
using System.Threading.Tasks;

namespace UUIDproducer
{
    class Program
    {
        static void Main(string[] args)
        {

            //Consumer.getMessage();


            //Uuid.createEvent();


            //Uuid.createMockEvent();
            //Uuid.updateMockEvent();
            //Uuid.updateMockEventFromFrontEnd();
            Uuid.createMockEvent();
            /*
            Task task = new Task(() => Producer.send());
            task.Start();
            Consumer.getMessage();

            */
        }
    }
}
