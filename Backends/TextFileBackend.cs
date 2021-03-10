using gpstrackerd.Models;
using System;
using System.IO;

namespace gpstrackerd.Backends
{
    class TextFileBackend : IBackendClient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string OutputFilename { get; set; }

        public TextFileBackend(string OutputFilename)
        {
            this.OutputFilename = OutputFilename;
            if (OutputFilename == "")
            {
                log.Warn("TextFileBackend initialised without output file - backend disabled.");
            }
            else
            {
                log.InfoFormat("TextFileBackend initialised using file name '{0}'.", OutputFilename);
            }

        }

        public TextFileBackend() : this("")
        {

        }

        public void HandleTrackingInfoReceived(TrackerMessage message)
        {
            if (message != null)
            {
                string logLine = string.Format("[{0}] ({1})  {2},{3} - direction: {4} speed: {5}", DateTime.Now, message.DeviceID, message.Lat, message.Long, message.Direction, message.Speed);
                if (OutputFilename != "")
                {
                    using (StreamWriter sw = File.AppendText(OutputFilename))
                    {
                        sw.WriteLine(logLine);
                    }
                }
            }
        }
    }
}
