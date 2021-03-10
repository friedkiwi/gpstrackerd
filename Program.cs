using gpstrackerd.Backends;
using gpstrackerd.Models;
using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace gpstrackerd
{
    static class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
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
                    switch (backendConfig.BackendName.ToLowerInvariant())
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
            }
            catch (IOException ioex)
            {
                log.ErrorFormat("Error while opening config file: {0}", ioex.Message);
            }
            catch (SyntaxErrorException syntaxerr)
            {
                log.ErrorFormat("Syntax error in config file: {0}", syntaxerr.Message);
            }
        }
    }
}
