using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Routing;
using RoboChat.Common.Messages;
using RoboChat.Common.Messages.Client;
using RoboChat.Common.Model;

namespace RoboChat.Server.Actors
{
    public class ChatHubActor : TypedActor, IHandle<FindNearestHubMessage>, 
        IHandle<HubNameResponseMessage>,
        IHandle<ListRoomsMessage>,
        IHandle<ConnectToHubMessage>,
        IHandle<ClientRequestedRoomsListMessage>,
        IHandle<ClientCreateRoomMessage>
    {
        private string region;
        private string _idOfThisHub = Guid.NewGuid().ToString();

        private readonly Dictionary<string, ChatRoom> rooms = new Dictionary<string, ChatRoom>();
        private Dictionary<string, IActorRef> _clients = new Dictionary<string, IActorRef>();
        private string _lobbyRoomId;
        private ActorSelection _parentBroadcaster;
        private ActorSelection _parentChatServerCoordinator;
        private Dictionary<string,string> _clientIdsForTheirPaths = new Dictionary<string, string>();

        protected override void PreStart()
        {
            _lobbyRoomId = Guid.NewGuid().ToString();
            _parentChatServerCoordinator = Context.ActorSelection("../.."); // real parent (above broadcast pool)
            var selection = Context.ActorSelection("../.."); // real parent (above broadcast pool)
            selection.Tell(new GetHubNameMessage());

            _parentBroadcaster = Context.ActorSelection("../"); // broadcast pool

            Console.WriteLine("ChatHubActor PreStart "  + _idOfThisHub);
            base.PreStart();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            base.PreRestart(reason, message);
        }

        public ChatHubActor()
        {
            Console.WriteLine("ChatHubActor created " + _idOfThisHub);
        }

        public void Handle(FindNearestHubMessage message)
        {
            Sender.Tell(new NearestHubResponseMessage(_idOfThisHub, region));
        }

        public void Handle(HubNameResponseMessage message)
        {
            region = message.HubName;
            Console.WriteLine("ChatHubActor got roomName " + region + " creating lobby");
            CreateRoom(_lobbyRoomId, "CHATHUB " + region, _idOfThisHub, this.Self);
        }

        private ChatRoom CreateRoom(string roomId, string roomName, string ownerId, IActorRef owner, List<IActorRef> participants = null )
        {
            var room = new ChatRoom()
            {
                Id = roomId,
                Name = roomName,
                OwnerId = ownerId,
                Owner = owner,
                Participants = participants ?? new List<IActorRef>()
            };

            rooms.Add(roomId, room);

            Console.WriteLine("RoomId: {0} named: {1} created for ownerid: {2}", room.Id, room.Name, room.OwnerId);

            _parentChatServerCoordinator.Tell(new RoomsHaveChangedMessage(_idOfThisHub, GetLocalRooms()));

            return room;
        }

        public void Handle(ListRoomsMessage message)
        {
            Sender.Tell(new ListRoomsResponseMessage(_idOfThisHub, GetLocalRooms()));
        }

        private List<ChatRoomDetails> GetLocalRooms()
        {
            return rooms.Values.Select(a => new ChatRoomDetails(a.Id, a.Name, a.Participants.Count)).ToList();
        }

        public void Handle(ConnectToHubMessage message)
        {
            var clientId = Guid.NewGuid().ToString();
            _clients.Add(clientId, Sender);
            _clientIdsForTheirPaths.Add(Sender.Path.ToStringWithUid(), clientId);
            rooms[_lobbyRoomId].Participants.Add(Sender);
            Sender.Tell(new ClientHubConnectAcknowledgement(clientId));
            var msg = new SendRoomChatMessage(_lobbyRoomId, "HUB", string.Format("{0} has joined the room LOBBY", message.ClientNick),DateTime.UtcNow);
            foreach (var participant in rooms[_lobbyRoomId].Participants)
            {
                participant.Tell(msg);
            }
        }

        public void Handle(ClientRequestedRoomsListMessage message)
        {
            _parentChatServerCoordinator.Tell(message, Sender);
        }

        public void Handle(ClientCreateRoomMessage message)
        {
            var ownerId = _clientIdsForTheirPaths[Sender.Path.ToStringWithUid()];
            CreateRoom(Guid.NewGuid().ToString(), message.RoomName, ownerId, Sender, new List<IActorRef>() {Sender});
        }
    }
}