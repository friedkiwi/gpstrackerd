using gpstrackerd.Models;
using SharpKml.Base;
using SharpKml.Dom;
using System.IO;

namespace gpstrackerd.Backends
{
    class KmlBackend : IBackendClient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string KmlFile { get; set; }

        public KmlBackend()
        {
            
        }

        public KmlBackend(string KmlFile) : this()
        {
            this.KmlFile = KmlFile;

            if (KmlFile != "")
                log.InfoFormat("KmlBackend initialised using file name '{0}'.", KmlFile);
            else
                log.InfoFormat("KmlBackend initialised without a file name specified.", KmlFile);
        }

        public void HandleTrackingInfoReceived(TrackerMessage message)
        {
            log.Error("Cannot log to KML file when no KML file has been specified.");

            var point = new Point
            {
                Coordinate = new Vector(message.Lat, message.Long)
            };



            var placemark = new Placemark
            {
                Name = message.DeviceID,
                Time = new Timestamp
                {
                    When = message.Timestamp
                },
                Description = new Description
                {
                    Text = string.Format("Speed: {0}, Direction: {1}", message.Speed, message.Direction)
                },
                Geometry = point
            };

            var serializer = new Serializer();
            if (!File.Exists(KmlFile))
            {
                log.InfoFormat("Creating new KML file '{0}'.", KmlFile);
                var newKml = new Kml();
                newKml.Feature = new Folder
                {
                    Id = "f0",
                    Name = "gpstrackerd events"
                };
                serializer.Serialize(newKml);
                File.WriteAllText(KmlFile, serializer.Xml);
            }

            Parser parser = new Parser();
            parser.ParseString(File.ReadAllText(KmlFile), false);

            var kml = (Kml) parser.Root;
            var folder = (Folder) kml.Feature;
            folder.AddFeature(placemark);

            serializer.Serialize(kml);
            File.WriteAllText(KmlFile, serializer.Xml);
            log.InfoFormat("KmlBackend appended record to file '{0}'.", KmlFile);

        }
    }
}
