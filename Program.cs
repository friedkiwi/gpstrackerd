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
                var listenAddress = "127.0.0.1";

                if (config.ListenAddress != "")
                    listenAddress = config.ListenAddress;

                TrackerListener trackerListener = new TrackerListener(listenAddress, config.Port);
                foreach (var backendConfig in config.Backends)
                {
                    switch (backendConfig.BackendName.ToLowerInvariant())
                    {
                        case "textfile":
                            var txtBackend = new TextFileBackend(backendConfig.BackendEndpoint);
                            trackerListener.TrackingInfoReceived += txtBackend.HandleTrackingInfoReceived;
                            break;
                        case "splunk":
                            var splunkBackend = new SplunkBackend(backendConfig.BackendEndpoint, backendConfig.AuthToken);
                            trackerListener.TrackingInfoReceived += splunkBackend.HandleTrackingInfoReceived;
                            break;
                        case "kml":
                            var kmlBackend = new KmlBackend(backendConfig.BackendEndpoint);
                            trackerListener.TrackingInfoReceived += kmlBackend.HandleTrackingInfoReceived;
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
