using System;
using Akka.Actor;

namespace IotMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public class IotApp
    {
        public static void Init()
        {
            using var system = ActorSystem.Create("iot-system");
            // Create top level supervisor
            var supervisor = system.ActorOf(IotSupervisor.Props(), "iot-supervisor");
            // Exit the system after ENTER is pressed
            Console.ReadLine();
        }
    }
}