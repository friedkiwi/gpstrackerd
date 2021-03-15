using gpstrackerd.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace gpstrackerd.Backends
{
    class SplunkBackend : IBackendClient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string AuthenticationToken { get; set; }
        public string Endpoint { get; set; }

        public SplunkBackend(string Endpoint, string AuthenticationToken)
        {
            this.Endpoint = Endpoint;
            this.AuthenticationToken = AuthenticationToken;
            log.InfoFormat("SplunkBackend initialised using endpoint '{0}'.", Endpoint);
        }
        public SplunkBackend() : this("", "")
        {

        }
        public void HandleTrackingInfoReceived(TrackerMessage message)
        {
            SendDataToSplunk(message).Wait();
        }

        private async Task SendDataToSplunk(TrackerMessage message)
        {
            log.DebugFormat("Sending tracker event from tracker {0} to endpoint {1}", message.DeviceID, Endpoint);
            string splunkObject = JsonConvert.SerializeObject(message);
            string postUrl = string.Format("{0}/services/collector/raw", Endpoint);

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Splunk", AuthenticationToken);
                await client.PostAsync(postUrl, new StringContent(splunkObject, Encoding.UTF8, "application/json"));
            } catch (Exception e)
            {
                log.ErrorFormat("Failed to send data to splunk: {0}", e.Message);
            }
        }
    }
}
