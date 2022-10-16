using System.Xml.Serialization;
using RabbitMQ.Client;
using System.Text;
using System;
using RabbitLogging.logging;

namespace RabbitLogging.logging
{


    /// <summary>
    /// Verbindungsklasse zum RabbitMQ Server
    /// </summary>
    class Connector
    {

        static IConnection Connection;

        static IModel Channel;

        public static string HostName { get; set; }

        private static bool StructuresDeclared;

        public static string EXCHANGE_NAME { get; set; }

        public static string EXCHANGE_TYPE { get; set; }

        static Connector()
        {
            EXCHANGE_NAME = "logging_router";
            EXCHANGE_TYPE = "topic";
        }


        /// <summary>
        /// Verbindet sich mit dem Server.
        /// Hier fallen ggf. Exceptions bei fehlerhafter Konfiguration an
        /// </summary>
        public static void Connect()
        {
            var factory = new ConnectionFactory() { HostName = Connector.HostName };
            Connector.Connection = factory.CreateConnection();
            Connector.Channel = Connector.Connection.CreateModel();
            Console.WriteLine("Connected to Server");
        }

        /// <summary>
        /// Stellt fest, ob die Verbindung noch hergestellt ist.
        /// </summary>
        /// <returns> IS Connected</returns>
        public static bool IsConnected()
        {
            return Connector.Channel != null && Connector.Channel.IsOpen && Connector.Connection != null && Connector.Connection.IsOpen;
        }

        /// <summary>
        /// Trennt die Verbindung zum Server
        /// </summary>
        public static void Disconnect()
        {
            if (Connector.Channel != null && Connector.Channel.IsOpen)
            {
                Connector.Channel.Close();
            }
            if (Connector.Connection != null && Connector.Connection.IsOpen)
            {
                Connector.Connection.Close();
            }
        }


        public static void DeclareStructures()
        {
            if (!Connector.IsConnected())
            {
                Connector.Connect();
            }
            Connector.Channel.ExchangeDeclare(exchange: EXCHANGE_NAME,
                type: EXCHANGE_TYPE,
                durable: true,
                autoDelete: true
                );
            Connector.StructuresDeclared = true;
        }

        /// <summary>
        /// Erstellt eine Log-Message und schickt sie an den Server.
        /// </summary>
        /// <param name="Text"> Inhalt der Nahricht</param>
        /// <param name="Severity"> Severity der Nachricht</param>
        /// <param name="SystemName"> Name des sendenden Systems</param>
        public static void write(string Text, Severity Severity, string SystemName)
        {
            Connector.write(new Message(Text, Severity, SystemName));
        }

        /// <summary>
        /// Erstellt eine Log-Message und schickt sie an den Server.
        /// </summary>
        /// <param name="Message"> Message</param>
        public static void write(Message Message)
        {
            if (!Connector.IsConnected())
            {
                Connector.Connect();
            }
            if (!Connector.StructuresDeclared)
            {
                Connector.DeclareStructures();
            }

            Console.WriteLine($"Conn: {Connector.IsConnected()}, Struc: {Connector.StructuresDeclared}");
            Console.WriteLine($"Model {Connector.Channel}");
            using (var sw = new System.IO.StringWriter())
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(Message));
                Serializer.Serialize(sw, Message);
                string routingKey = "logs";
                if (Message.Severity > Severity.INFO)
                {
                    routingKey += ".Important";
                }
                Console.WriteLine($"({EXCHANGE_NAME},{routingKey})");
                Connector.Channel.BasicPublish(exchange: EXCHANGE_NAME,
                                            routingKey: routingKey,
                                            basicProperties: null,
                                            body: Encoding.UTF8.GetBytes(sw.ToString()));
            }
        }

    }
}