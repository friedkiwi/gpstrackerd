using gpstrackerd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpstrackerd.Backends
{
    class MySqlBackend : IBackendClient
    {
        public void HandleTrackingInfoReceived(TrackerMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
