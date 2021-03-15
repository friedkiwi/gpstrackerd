using gpstrackerd.Models;
using SharpKml.Base;
using SharpKml.Dom;
using System.IO;

namespace gpstrackerd.Backends
{
    class KmlBackend : IBackendClient
    {
        public string KmlFile { get; set; }

        public KmlBackend()
        {

        }

        public KmlBackend(string KmlFile)
        {
            this.KmlFile = KmlFile;
        }

        public void HandleTrackingInfoReceived(TrackerMessage message)
        {
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

        }
    }
}
