using System.Xml.Serialization;
using RabbitMQ.Client;
using System.Text;
using System;

namespace Base.logging
{


    /// <summary>
    /// Verbindungsklasse zum RabbitMQ Server
    /// </summary>
    public class Connector: AbstractConnector
    {


        public Connector(string HostName) : base(HostName)
        {

        }

        /// <summary>
        /// Erstellt eine Log-Message und published sie an einen Exchange.
        /// </summary>
        /// <param name="Text"> Inhalt der Nahricht</param>
        /// <param name="Severity"> Severity der Nachricht</param>
        /// <param name="SystemName"> Name des sendenden Systems</param>
        /// <param name="Exchange"> Name des Exchanges</param>
        public void write(string Text, Severity Severity, string SystemName,string Exchange)
        {
            this.write(new Message(Text, Severity, SystemName),Exchange);
        }

        /// <summary>
        /// Erstellt eine Log-Message und published sie an einen Exchange.
        /// </summary>
        /// <param name="Message"> Log Nachricht</param>
        /// <param name="Exchange"> Name des Exchanges</param>
        public void write(Message Message,string Exchange)
        {
            if (!this.IsConnected())
            {
                this.Connect();
            }
            if(this.Exchanges.FindAll((exchange) => exchange.Name == Exchange).Count == 0)
            {
                Console.WriteLine(this.Exchanges.FindAll((exchange) => exchange.Name == Exchange).Count);
                Console.WriteLine($"Connector.Write() called witd Exchange = {Exchange}");
                for(int i = 0; i< this.Exchanges.Count; i++)
                {
                    Console.WriteLine($"{i}. Exchange: {this.Exchanges[i]}");
                }
                throw new ArgumentException("Der gennante Exchange ist nicht in den Deklarierten Exchanges enthalten!");
            }
            using (var sw = new System.IO.StringWriter())
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(Message));
                Serializer.Serialize(sw, Message);
                string routingKey = Message.GetRoutingKey();
                this.Channel.BasicPublish(exchange: Exchange,
                                            routingKey: routingKey,
                                            basicProperties: null,
                                            body: Encoding.UTF8.GetBytes(sw.ToString()));
            }
        }
     
    }
}