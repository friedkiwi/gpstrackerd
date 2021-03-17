using gpstrackerd.Models;
using System;
using MySqlConnector;

namespace gpstrackerd.Backends
{
    class MySqlBackend : IBackendClient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string ConnectionString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public MySqlBackend()
        {
            log.Info("Initialised MySQL backend"); 
        }

        public MySqlBackend(string ConnectionString, string Username, string Password) : this()
        {
            this.ConnectionString = ConnectionString;
            this.Username = Username;
            this.Password = Password;
        }

        public void HandleTrackingInfoReceived(TrackerMessage message)
        {
            var csb = new MySqlConnectionStringBuilder(this.ConnectionString);
            csb.UserID = this.Username;
            csb.Password = this.Password;
            using(var conn = new MySqlConnection(csb.ToString()))
            {
                try
                {
                    conn.Open();
                } catch (Exception e)
                {
                    log.ErrorFormat("Error while connecting to MySQL instance: {0}", e.Message);
                    return;
                }
                
                log.InfoFormat("Inserting tracker record into MySQL {0} server.", conn.ServerVersion);

                var insertionCommand = conn.CreateCommand();
                insertionCommand.CommandText = "INSERT INTO tracker_events (device_id, direction, latitude, longitude, speed) VALUES (@device_id, @direction, @latitude, @longitude, @speed)";
                insertionCommand.Parameters.Add(new MySqlParameter("device_id", message.DeviceID));
                insertionCommand.Parameters.Add(new MySqlParameter("direction", message.Direction));
                insertionCommand.Parameters.Add(new MySqlParameter("latitude", message.Lat));
                insertionCommand.Parameters.Add(new MySqlParameter("longitude", message.Long));
                insertionCommand.Parameters.Add(new MySqlParameter("speed", message.Speed));

                try
                {
                    insertionCommand.ExecuteNonQuery();
                    
                }catch (Exception e)
                {
                    log.ErrorFormat("Failed to insert record into MySQL database: {0}", e.Message);
                }
            }
        }
    }
}
