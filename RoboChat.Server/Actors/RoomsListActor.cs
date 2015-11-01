using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using RoboChat.Common.Messages;
using RoboChat.Common.Messages.Client;

namespace RoboChat.Server.Actors
{
    public class RoomsListActor : TypedActor,
        IHandle<ClientRequestedRoomsListMessage>,
        IHandle<ListRoomsResponseMessage>,
        IHandle<RoomsHaveChangedMessage>
    {
        private Dictionary<string, List<ChatRoomDetails>> _hubsRooms = new Dictionary<string, List<ChatRoomDetails>>();
        protected override void PreStart()
        {
            var selection = Context.ActorSelection("../.."); // real parent (above broadcast pool)
            selection.Tell(new ListRoomsMessage());
        }

        public void Handle(ClientRequestedRoomsListMessage message)
        {
            Sender.Tell(new ClientRoomListResponseMessage(_hubsRooms.Values.SelectMany(a => a)));
        }

        public void Handle(ListRoomsResponseMessage message)
        {
            UpdateRoomsForHub(message.HubId, message.Rooms);
        }

        private void UpdateRoomsForHub(string hubId, IEnumerable<ChatRoomDetails> rooms)
        {
            if (!_hubsRooms.ContainsKey(hubId))
            {
                _hubsRooms.Add(hubId, rooms.ToList());
            }
            else
            {
                _hubsRooms[hubId] = rooms.ToList();
            }
        }

        public void Handle(RoomsHaveChangedMessage message)
        {
            Console.WriteLine("Updating rooms for hub {0}", message.HubId);
            UpdateRoomsForHub(message.HubId, message.Rooms);
        }
    }
}