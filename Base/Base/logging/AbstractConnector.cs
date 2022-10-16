using System;
using System.Collections.Generic;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace Base.logging
{
    public abstract class AbstractConnector:IDisposable
    {
        /// <summary>
        /// Verbindung zum RabbitMQ-Server
        /// </summary>
        protected IConnection Connection;

        /// <summary>
        /// Model von RabbitMQ
        /// </summary>
        protected IModel Channel;

        /// <summary>
        /// Server HostName
        /// </summary>
        protected string HostName { get; set; }

        /// <summary>
        /// Deklarierte Exchanges
        /// </summary>
        protected List<(string Name,string Type)> Exchanges;

        /// <summary>
        /// Deklarierte Queues
        /// </summary>
        protected List<(string Name, string Exchange)> Queues;

        /// <summary>
        /// Kosntruktor für einen einfachen Connctor
        /// </summary>
        /// <param name="HostName"> Hostname des RabbitMQ-Servers</param>
        public AbstractConnector(string HostName)
        {
            this.HostName = HostName;
            this.Exchanges = new List<(string,string)>();
            this.Queues = new List<(string,string)>();
        }

        /// <summary>
        /// Stellt eine Verbindung zum RabbitMQ-Server Her
        /// </summary>
        public void Connect() {
            ConnectionFactory Factory = new ConnectionFactory() { HostName = this.HostName };
            this.Connection = Factory.CreateConnection();
            this.Channel = this.Connection.CreateModel();
        }

        /// <summary>
        /// Erstellt einen Exchange mit dem Übergebenen Namen und dem Übergebenen Typen
        /// </summary>
        /// <param name="ExchangeName"> Name des Exchanges</param>
        /// <param name="ExchangeType"> Typ des Exchanges</param>
        public void DeclareExchange(string ExchangeName,string ExchangeType)
        {
            this.DeclareExchange(ExchangeName, ExchangeType, Durable: false, AutoDelete: true, null);
        }

        /// <summary>
        /// Detaillierte Möglichkeit einen Exchange zu erstellen und Parameter zu übergeben
        /// </summary>
        /// <param name="ExchangeName"> Name es Exchanges</param>
        /// <param name="ExchangeType"> Typ des Exchanges</param>
        /// <param name="Durable"></param>
        /// <param name="AutoDelete"></param>
        /// <param name="Args"></param>
        public void DeclareExchange(string ExchangeName, string ExchangeType, bool Durable, bool AutoDelete, IDictionary<string, object> Args)
        {
            this.Channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType, durable: Durable, autoDelete: AutoDelete, arguments: Args);
            this.Exchanges.Add((ExchangeName, ExchangeType));
        }

        /// <summary>
        /// Erstellt eine Queue und bindet sie an den genannten Exchange mit dem übergebenen RoutingKey
        /// </summary>
        /// <param name="ExchangeName"> Name des Exchanges</param>
        /// <param name="RoutingKey"> RoutingKey für die Queue</param>
        /// <returns> Name der erzeugten Queue</returns> 
        public string BindQueueToExchange(string ExchangeName,string RoutingKey)
        {
            return this.BindQueueToExchange(ExchangeName, RoutingKey, Durable: false, Exclusive: true, AutoDelete: true, Args: null);
        }

        /// <summary>
        /// Detaillierte Möglichkeit eine Queue an einen Exchange zu binden
        /// </summary>
        /// <param name="ExchangeName"> Name des Exchanges</param>
        /// <param name="RoutingKey"> RoutingKey für die Queue</param>
        /// <param name="Durable"></param>
        /// <param name="Exclusive"></param>
        /// <param name="AutoDelete"></param>
        /// <param name="Args"></param>
        /// <returns> Name der erzeugten Queue</returns>
        public string BindQueueToExchange(string ExchangeName, string RoutingKey,bool Durable, bool Exclusive,bool AutoDelete, IDictionary<string, object> Args)
        {
            string Queue = this.Channel.QueueDeclare(durable: Durable, exclusive: Exclusive, autoDelete: AutoDelete, arguments: Args);
            this.Channel.QueueBind(Queue, ExchangeName, RoutingKey);
            this.Queues.Add((Queue, ExchangeName));
            return Queue;
        }

        /// <summary>
        /// Fügt einen Consumer für das Erhalten von Messages hinzu
        /// </summary>
        /// <param name="Queue"> Name der Queue</param>
        /// <param name="consumer"> Eventhandler für Consumer</param>
        public void AddConsumer(string Queue,EventHandler<BasicDeliverEventArgs> consumer)
        {
            EventingBasicConsumer Consumer = new EventingBasicConsumer(this.Channel);
            Consumer.Received += consumer;
            this.Channel.BasicConsume(queue: Queue, autoAck: true, Consumer);
        }

        /// <summary>
        /// Stellt fest, ob der Connector mit dem RabbitMQ-Server verbunden ist und einsatzbereit ist
        /// </summary>
        /// <returns>true, wenn verbunden und einsatzbereit</returns>
        public bool IsConnected()
        {
            return this.Connection != null && this.Connection.IsOpen && this.Channel != null && this.Channel.IsOpen;
        }

        /// <summary>
        /// Enternt alle Exchanges, Queues, Channels und Connections
        /// </summary>
        public void Dispose()
        {
            foreach(var Exchange in this.Exchanges)
            {
                this.Channel.ExchangeDelete(Exchange.Name);
            }
            foreach(var Queue in this.Queues)
            {
                this.Channel.QueueDelete(Queue.Name);
            }

            if(this.Channel != null)
            {
                this.Channel.Close();
                this.Channel.Dispose();
                this.Channel = null;
            }
            if(this. Connection != null)
            {
                this.Connection.Close();
                this.Connection.Dispose();
                this.Connection = null;
            }
        }


    }
}
