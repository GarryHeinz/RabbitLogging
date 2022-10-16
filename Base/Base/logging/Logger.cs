using System;
using System.Collections.Generic;

namespace RabbitLogging.logging
{
    public class Logger
    {
        private static Logger? Instance;

        public string HostName { get; set; }

        public Severity DefaultSeverity { get; set; }
        public string SystemName { get; set; }


        private Logger()
        {
            this.DefaultSeverity = Severity.INFO;
            this.SystemName = "";
            this.HostName = "localhost";
            Connector.HostName = this.HostName;
        }

        public static Logger GetInstance()
        {
            if (Logger.Instance == null)
            {
                Logger.Instance = new Logger();
            }
            return Logger.Instance;
        }



        public void log(string Text)
        {
            this.log(Text, this.DefaultSeverity);
        }


        public void log(string Text, Severity Severity)
        {

            Console.WriteLine(this.HostName);
            if (!Connector.IsConnected())
            {
                Connector.Connect();

            }
            Console.WriteLine("Message To Connector");
            Connector.write(Text, Severity, this.SystemName);
        }

        public void CloseConnections()
        {

            Connector.Disconnect();
        }

    }
}
