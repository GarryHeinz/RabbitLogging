using System;
using System.IO;
namespace RabbitLogging.logging
{
    public class Message
    {
        public static readonly Version VERSION = new Version(1, 0);

        public string SystemName { get; set; }

        public string Text { get; set; }

        private DateTime Date { get; }

        public Severity Severity { get; set; }

        public Message()
        {
            this.Text = "";
            this.Severity = Severity.INFO;
            this.SystemName = "";
            this.Date = DateTime.Now;
        }

        public Message(string Text, Severity Severity, string SystemName)
        {
            this.Text = Text;
            this.Severity = Severity;
            this.Date = DateTime.Now;
            this.SystemName = SystemName;
        }

        public void WriteToFile()
        {
            string Path = this.GetPath();
            if (!File.Exists(Path))
            {
                using (StreamWriter sw = File.CreateText(Path))
                {
                    sw.WriteLine(this.GetContent());
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(Path))
                {
                    sw.WriteLine(this.GetContent());
                }
            }

        }

        public string GetContent()
        {
            return this.Date.ToString() + ":" + this.Text;
        }

        public string GetPath()
        {
            return this.SystemName + this.Date.ToShortDateString() + ".log";
        }
    }

    public enum Severity
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR

    }
}
