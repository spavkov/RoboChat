using System.Collections.Generic;
using Akka.Actor;

namespace RoboChat.Common.Model
{
    public class ChatRoom
    {
        public string Id { get;  set; }

        public string Name { get;  set; }

        public IActorRef Owner { get;  set; }

        public List<IActorRef> Participants { get;  set; }
    }
}