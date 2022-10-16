using System;
using System.IO;
namespace Base.logging
{
    /// <summary>
    /// Klasse, die Lognachrichten Repräsentiert
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Version des Formats der Lognachricht
        /// </summary>
        public static readonly Version VERSION = new Version(1, 0);

        /// <summary>
        /// Name des Systems
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Inhalt der Lognachricht
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Zeitpunkt des Erstellens der Lognachricht
        /// </summary>
        private DateTime Date { get; }

        /// <summary>
        /// Severity der Nachricht
        /// </summary>
        public Severity Severity { get; set; }

        /// <summary>
        /// Konstruktor zum deserialisiern in XML
        /// </summary>
        public Message()
        {
            this.Text = "";
            this.Severity = Severity.INFO;
            this.SystemName = "";
            this.Date = DateTime.Now;
        }

        /// <summary>
        /// Konstuktor zum erstellen einer Nachticht
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Severity"></param>
        /// <param name="SystemName"></param>
        public Message(string Text, Severity Severity, string SystemName)
        {
            this.Text = Text;
            this.Severity = Severity;
            this.Date = DateTime.Now;
            this.SystemName = SystemName;
        }

        /// <summary>
        /// Schreibt die Nachricht in eine Datei
        /// </summary>
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

        /// <summary>
        /// Gibt den Inhalt der Nachricht und den Zeitpunkt wieder
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            return this.Date.ToString() + ":" + this.Text;
        }

        /// <summary>
        /// Gibt den Pfad der Logdatei zurück
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            return this.SystemName + this.Date.ToShortDateString() + ".log";
        }

        /// <summary>
        /// Gibt die Severity als String zurück. Dies entspricht dem RoutingKey
        /// </summary>
        /// <returns></returns>
        public string GetRoutingKey()
        {
            switch (this.Severity)
            {
                case Severity.DEBUG:
                    return "DEBUG";
                case Severity.INFO:
                    return "INFO";
                case Severity.WARNING:
                    return "WARNING";
                case Severity.ERROR:
                    return "ERROR";
                default:
                    return "";
            }
        }
    }

    /// <summary>
    /// Typ für die Darstellung von Nachrichtentypen
    /// </summary>
    public enum Severity
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }
}
