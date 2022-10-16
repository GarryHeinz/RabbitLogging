using System;
using System.Collections.Generic;

namespace Base.logging
{
    /// <summary>
    /// Logger Klasse, für das schreiben von Lognachrichten an RabbitMQ
    /// </summary>
    public class Logger
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
        /// Verwendete Severity, wenn keine anderweitige angegeben ist
        /// </summary>
        public Severity DefaultSeverity { get; set; }

        /// <summary>
        /// Der Systemname, unter dem die Logmessage Angezeigt wird
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Konstuktor für Logger
        /// </summary>
        /// <param name="HostName"> Hostname für RabbitMQ-Server</param>
        /// <param name="SystemName"> Name des loggenden Systems</param>
        /// <param name="DefaultSeverity"> Default Severity</param>
        public Logger(string HostName = "localhost", string SystemName = "Not Named",Severity DefaultSeverity = Severity.INFO)
        {
            this.DefaultSeverity = DefaultSeverity;
            this.SystemName = SystemName;
            this.HostName = HostName;
            this.Connector = new Connector(this.HostName);
        }

        /// <summary>
        /// Initalisiert den Logger
        /// </summary>
        /// <param name="ExchangeName">Name des Exchanges</param>
        /// <param name="ExchangeType">Typ des Exchanges</param>
        public void init(string ExchangeName = "logging_router", string ExchangeType = "topic")
        {
            this.ExchangeName = ExchangeName;
            this.ExchangeType = ExchangeType;
            this.Connector.Connect();
            this.Connector.DeclareExchange(this.ExchangeName, this.ExchangeType);
        }

        /// <summary>
        /// Scheibt eine Lognachricht
        /// </summary>
        /// <param name="Text"> Inhalt der Nachricht</param>
        public void log(string Text)
        {
            this.log(Text, this.DefaultSeverity);
        }

        /// <summary>
        /// Schreibt eine Lognachricht mit Severity
        /// </summary>
        /// <param name="Text">Inhalt der Nachricht</param>
        /// <param name="Severity">Severity der Nachricht</param>
        public void log(string Text, Severity Severity)
        {
            if (!this.Connector.IsConnected())
            {
                throw new ArgumentException("Der Connector ist nicht Korrekt verbunden. Logger.init() aufgerufen?");
            }
            this.Connector.write(Text, Severity, this.SystemName,this.ExchangeName);
        }

        /// <summary>
        /// Trennt die Verbindung des Connectors
        /// </summary>
        public void CloseConnections()
        {
            this.Connector.Dispose();
        }

    }
}
