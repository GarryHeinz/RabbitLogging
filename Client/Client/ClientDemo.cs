using System;
using RabbitLogging.logging;
public class ClientDemo
{

    public static void Main(string[] args)
    {
        Logger l = Logger.GetInstance();
        l.DefaultSeverity = Severity.DEBUG;
        Console.WriteLine("Enter System Name:");
        l.SystemName = Console.ReadLine();
        string? input;
        Console.WriteLine("Enter Messages:");
        while ((input = Console.ReadLine()) != null)
        {
            l.log(input);
            Console.WriteLine("####### \n");
        }
        l.CloseConnections();
    }
}