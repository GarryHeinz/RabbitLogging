using RabbitLogging.logging;
using System;

namespace RabbitLogging
{
    class ServerDemo
    {

        public static void Main(string[] args)
        {
            ServerLogger.Connect("localhost");
            ServerLogger.DeclareStructures();
            ServerLogger.Consume(ToConsole: true);
            Console.WriteLine("Waiting for Messages...");
            Console.WriteLine(ServerLogger.IsConnected());
            Console.ReadLine();

        }
    }
}
