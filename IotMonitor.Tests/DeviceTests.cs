using System;
using Akka.TestKit.NUnit;
using FluentAssertions;
using NUnit.Framework;

namespace IotMonitor.Tests
{
    public class DeviceTests : TestKit
    {
        [Test]
        public void Device_actor_must_reply_with_empty_reading_if_no_temperature_is_known()
        {
            var probe = CreateTestProbe();
            var deviceActor = Sys.ActorOf(Device.Props("group", "device"));

            deviceActor.Tell(new ReadTemperature(42), probe.Ref);
            var response = probe.ExpectMsg<RespondTemperature>();
            response.RequestId.Should().Be(42);
            response.Value.Should().Be(null);
        }
       
        [Test]
        public void Device_actor_must_reply_with_latest_temperature_reading()
        {
            var probe = CreateTestProbe();
            var deviceActor = Sys.ActorOf(Device.Props("group", "device"));

            deviceActor.Tell(new RecordTemperature(RequestId: 1, Value: 24.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 1);

            deviceActor.Tell(new ReadTemperature(2), probe.Ref);
            var response1 = probe.ExpectMsg<RespondTemperature>();
            response1.RequestId.Should().Be(2);
            response1.Value.Should().Be(24.0);

            deviceActor.Tell(new RecordTemperature(RequestId: 3, Value: 55.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 3);

            deviceActor.Tell(new ReadTemperature(4), probe.Ref);
            var response2 = probe.ExpectMsg<RespondTemperature>();
            response2.RequestId.Should().Be(4);
            response2.Value.Should().Be(55.0);
        }
        
       [Test]
public void Device_actor_must_reply_to_registration_requests()
{
    var probe = CreateTestProbe();
    var deviceActor = Sys.ActorOf(Device.Props("group", "device"));

    deviceActor.Tell(new RequestTrackDevice("group", "device"), probe.Ref);
    probe.ExpectMsg<DeviceRegistered>();
    probe.LastSender.Should().Be(deviceActor);
}

[Test]
public void Device_actor_must_ignore_wrong_registration_requests()
{
    var probe = CreateTestProbe();
    var deviceActor = Sys.ActorOf(Device.Props("group", "device"));

    deviceActor.Tell(new RequestTrackDevice("wrongGroup", "device"), probe.Ref);
    probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));

    deviceActor.Tell(new RequestTrackDevice("group", "Wrongdevice"), probe.Ref);
    probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));
}
    }
}