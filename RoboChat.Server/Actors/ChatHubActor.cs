using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using RoboChat.Common.Messages;
using RoboChat.Common.Model;

namespace RoboChat.Server.Actors
{
    public class ChatHubActor : TypedActor, IHandle<FindNearestHubMessage>, 
        IHandle<HubNameResponseMessage>,
        IHandle<ListRoomsMessage>
    {
        private string region;
        private string id = Guid.NewGuid().ToString();

        private Dictionary<string, ChatRoom> _rooms = new Dictionary<string, ChatRoom>(); 

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

            var newRoomId = Guid.NewGuid().ToString();
            _rooms.Add(newRoomId, new ChatRoom()
            {
                Id = newRoomId,
                Name = "CHATHUB " + region,
                Owner = this.Self,
                Participants = new List<IActorRef>()
            });
        }

        public void Handle(ListRoomsMessage message)
        {
            Sender.Tell(new ListRoomsResponseMessage(GetLocalRooms())
            {
            });
        }

        private List<ChatRoomDetails> GetLocalRooms()
        {
            return _rooms.Values.Select(a => new ChatRoomDetails(a.Id, a.Name, a.Participants.Count)).ToList();
        }
    }
}