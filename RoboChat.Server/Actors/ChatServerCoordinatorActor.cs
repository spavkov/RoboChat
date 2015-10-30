using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Akka.Actor;
using Akka.Routing;
using RoboChat.Common.Messages;

namespace RoboChat.Server.Actors
{
    public class ChatServerCoordinatorActor : TypedActor, IHandle<FindNearestHubMessage>, IHandle<GetHubNameMessage>, IHandle<ListRoomsMessage>
    {
        public ChatServerCoordinatorActor()
        {
            Console.WriteLine("ChatServerCoordinatorActor created");
            _rnd = new Random();
        }

        private SupervisorStrategy strategy = 
            new OneForOneStrategy(2, TimeSpan.FromSeconds(1),  
                x =>
                {
                //Maybe we consider ArithmeticException to not be application critical
                //so we just ignore the error and keep going.
                if (x is ArithmeticException) return Directive.Resume;

                //Error that we cannot recover from, stop the failing actor
                else if (x is NotSupportedException) return Directive.Stop;

                //In all other cases, just restart the failing actor
                else return Directive.Restart;
                });

        private IActorRef _hubs;


        protected override void PreStart()
        {
            var props = Props.Create<ChatHubActor>().WithRouter(new BroadcastPool(10, null, strategy, null));
            _hubs = Context.ActorOf(props, "hubs");
        }

        private string GetHubName()
        {
            return HubNames[_rnd.Next(HubNames.Count())];
        }

        private List<string> HubNames = new List<string>()
        {
            "1",
            "2",
            "3"
        };

        private Random _rnd;


        public void Handle(FindNearestHubMessage message)
        {
            _hubs.Forward(message);
        }

        public void Handle(GetHubNameMessage message)
        {
            Sender.Tell(new HubNameResponseMessage(GetHubName()));
        }

        public void Handle(ListRoomsMessage message)
        {
            _hubs.Forward(message);
        }
    }
}