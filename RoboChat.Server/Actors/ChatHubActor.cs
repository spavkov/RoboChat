using System;
using Akka.Actor;
using RoboChat.Common.Messages;

namespace RoboChat.Server.Actors
{
    public class ChatHubActor : TypedActor, IHandle<FindNearestHubMessage>, IHandle<HubNameResponseMessage>
    {
        private string region;
        private string id = Guid.NewGuid().ToString();

        protected override void PreStart()
        {
            var selection = Context.ActorSelection("../.."); // parent
            selection.Tell(new GetHubNameMessage());
            Console.WriteLine("ChatHubActor PreStart "  + id);
            base.PreStart();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            base.PreRestart(reason, message);
        }

        public ChatHubActor()
        {
            Console.WriteLine("ChatHubActor created " + id);
        }

        public void Handle(FindNearestHubMessage message)
        {
            Sender.Tell(new NearestHubResponseMessage(id, region));
        }

        public void Handle(HubNameResponseMessage message)
        {
            region = message.HubName;
            Console.WriteLine("ChatHubActor got name " + region);
        }
    }
}