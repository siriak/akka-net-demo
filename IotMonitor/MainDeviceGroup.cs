using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;

namespace IotMonitor
{
    public static partial class MainDeviceGroup
    {
        public record RequestTrackDevice(string GroupId, string DeviceId);

        public sealed class DeviceRegistered
        {
            public static DeviceRegistered Instance { get; } = new();
            private DeviceRegistered() { }
        }

        public class DeviceManager : UntypedActor
        {
            private Dictionary<string, IActorRef> groupIdToActor = new();
            private Dictionary<IActorRef, string> actorToGroupId = new();

            protected override void PreStart() => Log.Info("DeviceManager started");
            protected override void PostStop() => Log.Info("DeviceManager stopped");

            protected ILoggingAdapter Log { get; } = Context.GetLogger();

            protected override void OnReceive(object message)
            {
                switch (message)
                {
                    case RequestTrackDevice trackMsg:
                        if (groupIdToActor.TryGetValue(trackMsg.GroupId, out var actorRef))
                        {
                            actorRef.Forward(trackMsg);
                        }
                        else
                        {
                            Log.Info($"Creating device group actor for {trackMsg.GroupId}");
                            var groupActor = Context.ActorOf(DeviceGroup.Props(trackMsg.GroupId), $"group-{trackMsg.GroupId}");
                            Context.Watch(groupActor);
                            groupActor.Forward(trackMsg);
                            groupIdToActor.Add(trackMsg.GroupId, groupActor);
                            actorToGroupId.Add(groupActor, trackMsg.GroupId);
                        }
                        break;
                    case Terminated t:
                        var groupId = actorToGroupId[t.ActorRef];
                        Log.Info($"Device group actor for {groupId} has been terminated");
                        actorToGroupId.Remove(t.ActorRef);
                        groupIdToActor.Remove(groupId);
                        break;
                }
            }

            public static Props Props(string groupId) => Akka.Actor.Props.Create<DeviceManager>();
        }
    }
}