using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using RoboChat.Client.Actors;
using RoboChat.Common.Messages;
using RoboChat.Common.Messages.Client;

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
                var client = system.ActorOf(Props.Create<ChatClientActor>());

                Console.WriteLine("Enter command");

                while (true)
                {
                    var input = Console.ReadLine();
                    if (input.StartsWith("/"))
                    {
                        var parts = input.Split(' ');
                        var cmd = parts[0].ToLowerInvariant();
                        var rest = string.Join(" ", parts.Skip(1));

                        if (cmd == "/listrooms")
                        {
                            client.Tell(new ClientRequestedRoomsListMessage());
                        }

                        if (cmd == "/createroom")
                        {
                            client.Tell(new ClientCreateRoomMessage(rest));
                        }
                    }
                    else
                    {
                        /*
                        chatClient.Tell(new SayRequest()
                        {
                            Text = input,
                        });*/
                    }
                }

                Console.ReadLine();
            }
        }
    }
}
