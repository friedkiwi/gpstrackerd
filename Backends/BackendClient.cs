using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gpstrackerd.Models;

namespace gpstrackerd.Backends
{
    interface BackendClient
    {
        void HandleTrackingInfoReceived(TrackerMessage message);
    }
}
