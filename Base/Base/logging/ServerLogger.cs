using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System;

namespace Base.logging
{
    public class ServerLogger
    {

        /// <summary>
        /// Schnittstelle zur RabbitMQ
        /// </summary>
        private Connector Connector;

        /// <summary>
        /// Verwendeter HostName
        /// </summary>
        private string HostName;

        /// <summary>
        /// Name des verwendeten Exchanges
        /// </summary>
        private string ExchangeName;

        /// <summary>
        /// Verwendeter Exchangtyp
        /// </summary>
        private string ExchangeType;

        /// <summary>
        /// Name der Deklarierten Queue
        /// </summary>
        private string QueueName;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="HostName"> Hostname des RabbitMQ-Servers</param>
        public ServerLogger(string HostName = "localhost")
        {
            this.HostName = HostName;
            this.QueueName = "";
            this.Connector = new Connector(HostName);
        }

        /// <summary>
        /// Initialisiert den Logger
        /// </summary>
        /// <param name="ExchangeName"> Name des Exchanges</param>
        /// <param name="ExchangeType"> Typ des Exchanges</param>
        /// <param name="RoutingKey"> Key auf den abonniert werden soll</param>
        public void init(string ExchangeName = "logging_router", string ExchangeType = "topic",string RoutingKey = "#")
        {
            this.ExchangeName = ExchangeName;
            this.ExchangeType = ExchangeType;
            this.Connector.Connect();
            this.Connector.DeclareExchange(this.ExchangeName, this.ExchangeType);
            this.QueueName = this.Connector.BindQueueToExchange(this.ExchangeName, RoutingKey);
            this.Consume();
        }

        /// <summary>
        /// Fügt den Eventhandler als Consumer hinzu
        /// </summary>
        private void Consume()
        {
            EventHandler<BasicDeliverEventArgs> handler = (model, ea) =>
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
            };
            this.Connector.AddConsumer(this.QueueName,handler);
        }

    }
}