using System;
using Akka.Actor;
using RoboChat.Common.Messages;

namespace RoboChat.Client.Actors
{
    public class ChatClientActor : TypedActor, 
        IHandle<NearestHubResponseMessage>,
        IHandle<ListRoomsMessage>,
    IHandle<ListRoomsResponseMessage>
    {
        private readonly ActorSelection _server = Context.ActorSelection("akka.tcp://RoboChatServer@localhost:8081/user/ChatServerCoordinator");
        private IActorRef _hub;

        protected override void PreStart()
        {
            Console.WriteLine("Requesting the hub");
            _server.Tell(new FindNearestHubMessage());
        }

        public void Handle(NearestHubResponseMessage message)
        {
            if (_hub == null)
            {
                Console.WriteLine("Quickest hub: " + message.HubId + " " + message.HubRegion);
                _hub = Sender;
            }
            else
            {
                Console.WriteLine("Discarding the hub: " + message.HubId + " " + message.HubRegion);
            }
        }

        public void Handle(ListRoomsResponseMessage message)
        {
            foreach (var chatRoomDetailse in message.Rooms)
            {
                Console.WriteLine(chatRoomDetailse.Id  + " " + chatRoomDetailse.Name + " " + chatRoomDetailse.ParticipantsCount);
            }
        }

        public void Handle(ListRoomsMessage message)
        {
            _server.Tell(message);
        }
    }
}