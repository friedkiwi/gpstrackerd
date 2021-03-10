using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpstrackerd.Backends;
using gpstrackerd.Models;

namespace gpstrackerd
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            log.Info("Starting gpstrackerd");

            var textBackend = new TextFileBackend("tracker.log");

            TrackerListener tl = new TrackerListener("0.0.0.0", 18000);
            tl.TrackingInfoReceived += textBackend.HandleTrackingInfoReceived;
            tl.StartListener();
        }
    }
}
