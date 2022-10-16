using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System;

namespace RabbitLogging.logging
{
    public class ServerLogger
    {

        static IConnection Connection;

        static IModel Channel;

        private static string HostName;

        private static bool StructuresDeclared;

        private static string QueueName;

        public static string EXCHANGE_NAME { get; set; }

        public static string EXCHANGE_TYPE { get; set; }

        static ServerLogger()
        {
            EXCHANGE_NAME = "logging_router";
            EXCHANGE_TYPE = "topic";
        }


        /// <summary>
        /// Verbindet sich mit dem Server.
        /// Hier fallen ggf. Exceptions bei fehlerhafter Konfiguration an
        /// </summary>
        public static void Connect(string HostName)
        {
            var factory = new ConnectionFactory() { HostName = HostName };
            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            Console.WriteLine("Connected to Server");
        }

        /// <summary>
        /// Stellt fest, ob die Verbindung noch hergestellt ist.
        /// </summary>
        /// <returns> IS Connected</returns>
        public static bool IsConnected()
        {
            return Channel != null && Channel.IsOpen && Connection != null && Connection.IsOpen;
        }

        /// <summary>
        /// Trennt die Verbindung zum Server
        /// </summary>
        public static void Disconnect()
        {
            if (Channel != null && Channel.IsOpen)
            {
                Channel.Close();
            }
            if (Connection != null && Connection.IsOpen)
            {
                Connection.Close();
            }
        }
        public static void DeclareStructures()
        {
            Channel.ExchangeDeclare(exchange: EXCHANGE_NAME,
                type: EXCHANGE_TYPE,
                durable: true,
                autoDelete: true);
            QueueName = Channel.QueueDeclare(durable: false,
                exclusive: false,
                autoDelete: true).QueueName;
            Channel.QueueBind(QueueName, EXCHANGE_NAME, "logs.*");
            Console.WriteLine($"{EXCHANGE_NAME}, logs.*");
            StructuresDeclared = true;
        }

        public static void Consume(bool ToConsole)
        {
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += (model, ea) =>
            {
                string body = Encoding.UTF8.GetString(ea.Body.ToArray());
                XmlSerializer Serializer = new XmlSerializer(typeof(Message));
                MemoryStream MemoryStream = new MemoryStream(ea.Body.ToArray());
                Message? m = Serializer.Deserialize(MemoryStream) as Message;
                if (m == null)
                {
                    throw new Exception("Message Null! No Content to be Parsed!");
                }
                m.WriteToFile();
                if (ToConsole)
                {
                    Console.WriteLine(m.GetPath() + " -> " + m.GetContent());
                }
            };
            ServerLogger.Channel.BasicConsume(QueueName, true, consumer: consumer);
            Console.WriteLine($"{QueueName}, {consumer.IsRunning}");
        }

    }
}