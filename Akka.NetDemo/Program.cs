using System;
using Akka.Actor;

namespace Akka.NetDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("system-name");
            var firstRef = system.ActorOf(Props.Create<PrintMyActorRefActor>(), "first-actor");
            Console.WriteLine($"First: {firstRef}");
            firstRef.Tell("printit", ActorRefs.NoSender);
            
            var first = system.ActorOf(Props.Create<StartStopActor1>(), "first");
            first.Tell("stop"); 
            
            Console.ReadLine();
        }
        
    }
}