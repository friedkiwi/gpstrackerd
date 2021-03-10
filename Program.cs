using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpstrackerd.Backends;
using gpstrackerd.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Core;
using System.IO;

namespace gpstrackerd
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            /*log.Info("Starting gpstrackerd");

            var textBackend = new TextFileBackend("tracker.log");

            TrackerListener tl = new TrackerListener("0.0.0.0", 18000);
            tl.TrackingInfoReceived += textBackend.HandleTrackingInfoReceived;
            tl.StartListener();*/
            if (args.Length != 1)
            {
                Console.WriteLine("usage: gpstrackerd config.yaml");
                return;
            }
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            try
            {
                var config = deserializer.Deserialize<ConfigFileModel>(File.ReadAllText(args[0]));

                TrackerListener trackerListener = new TrackerListener("0.0.0.0", config.Port);
                foreach (var backendConfig in config.Backends)
                {
                    switch(backendConfig.BackendName.ToLowerInvariant())
                    {
                        case "textfile":
                            var backend = new TextFileBackend(backendConfig.BackendEndpoint);
                            trackerListener.TrackingInfoReceived += backend.HandleTrackingInfoReceived;
                            break;
                        default:
                            log.WarnFormat("Invalid backend name specified: {0}", backendConfig.BackendName);
                            break;
                    }
                }
                trackerListener.StartListener();
            } catch (IOException ioex)
            {
                log.ErrorFormat("Error while opening config file: {0}", ioex.Message);
            } catch (SyntaxErrorException syntaxerr)
            {
                log.ErrorFormat("Syntax error in config file: {0}", syntaxerr.Message);
            }
        }
    }
}
