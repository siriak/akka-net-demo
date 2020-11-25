using System;
using Akka.Actor;

namespace Akka.NetDemo
{
    public class StartStopActor1 : UntypedActor
    {
        protected override void PreStart()
        {
            Console.WriteLine("first started");
            Context.ActorOf(Props.Create<StartStopActor2>(), "second");
        }

        protected override void PostStop() => Console.WriteLine("first stopped");

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case "stop":
                    Context.Stop(Self);
                    break;
            }
        }
    }
}