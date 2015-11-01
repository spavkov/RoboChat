using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using RoboChat.Common.Messages;
using RoboChat.Common.Messages.Client;

namespace RoboChat.Client.Actors
{
    public class ChatClientActor : TypedActor, 
        IHandle<NearestHubResponseMessage>,
        IHandle<ClientRequestedRoomsListMessage>,
        IHandle<ClientRoomListResponseMessage>,
        IHandle<SendRoomChatMessage>,
        IHandle<ClientHubConnectAcknowledgement>,
        IHandle<ClientCreateRoomMessage>
    {
        private readonly ActorSelection _server = Context.ActorSelection("akka.tcp://RoboChatServer@localhost:8081/user/ChatServerCoordinator");
        private IActorRef _hub;
        private string _id;
        private Dictionary<string, ChatRoomDetails> _knownRooms = new Dictionary<string, ChatRoomDetails>();

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
                Console.WriteLine("Connecting to hub " + message.HubId + " " + message.HubRegion);
                _hub.Tell(new ConnectToHubMessage("slobo"));
            }
            else
            {
                Console.WriteLine("Discarding the hub: " + message.HubId + " " + message.HubRegion);
            }
        }

        public void Handle(ClientRoomListResponseMessage message)
        {
            _knownRooms = message.Rooms.ToDictionary(a => a.Id, b => b);
            foreach (var chatRoomDetailse in message.Rooms)
            {
                Console.WriteLine(chatRoomDetailse.Id  + " " + chatRoomDetailse.Name + " " + chatRoomDetailse.ParticipantsCount);
            }
        }

        public void Handle(ClientRequestedRoomsListMessage message)
        {
            Console.WriteLine("Requesting rooms");
            _hub.Tell(message);
        }

        public void Handle(SendRoomChatMessage message)
        {
            var roomName = _knownRooms.ContainsKey(message.RoomId) ? _knownRooms[message.RoomId].Name : "???";
            Console.WriteLine("[{0}] [1] {2}: {3}", roomName, message.SentUtc, message.SenderNick, message.MessageText);
        }

        public void Handle(ClientHubConnectAcknowledgement message)
        {
            _id = message.ClientId;
            Console.WriteLine("Got id {0}", _id);
        }

        public void Handle(ClientCreateRoomMessage message)
        {
            _hub.Tell(message);
        }
    }
}