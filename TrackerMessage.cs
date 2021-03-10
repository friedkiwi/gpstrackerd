using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpstrackerd
{
    class TrackerMessage
    {
        public string DeviceID { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public DateTime Timestamp { get; set; }
        public double Speed { get; set; }
        public int Direction { get; set; }

        public TrackerMessage()
        {
            Timestamp = DateTime.Now;
        }

        public TrackerMessage(string DeviceID) : this()
        {
            this.DeviceID = DeviceID;
        }

        private double ParseLatLong(string gpslatlong, string dir, bool longitude = false)
        {
            double decimalResult;
            double degrees, minutes;
            if (longitude)
            {
                degrees = Convert.ToDouble(gpslatlong.Substring(0, 3));
                minutes = Convert.ToDouble(gpslatlong.Substring(3));
            }
            else
            {
                degrees = Convert.ToDouble(gpslatlong.Substring(0, 2));
                minutes = Convert.ToDouble(gpslatlong.Substring(2));
            }
            
            decimalResult = degrees + (minutes / 60);

            if (dir == "S" || dir == "W")
            {
                decimalResult = 0 - decimalResult;
            }

            return decimalResult;
        }

        public void SetLatLong(string GpsLat, string LatDir, string GpsLong, string LongDir)
        {
            Lat = ParseLatLong(GpsLat, LatDir);
            Long = ParseLatLong(GpsLong, LongDir, true);
        }

        public override string ToString()
        {
            return string.Format("Device ID: {0}, Latitude: {1}, Longitude: {2}, Speed: {3}, Direction: {4}", 
                this.DeviceID, this.Lat, this.Long, this.Speed, this.Direction);
        }
    }
}
