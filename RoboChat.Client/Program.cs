using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using RoboChat.Common.Messages;

namespace RoboChat.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
                akka {  
                    actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    }
                    remote {
                        helios.tcp {
                            transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
		                    applied-adapters = []
		                    transport-protocol = tcp
		                    port = 0
		                    hostname = localhost
                        }
                    }
                }");

            using (var system = ActorSystem.Create("RoboChatClient", config))
            {
                var selection = system.ActorSelection("akka.tcp://RoboChatServer@localhost:8081/user/ChatServerCoordinator");
                selection.Tell(new FindNearestHubMessage());
                Console.ReadLine();
            }
        }
    }
}
