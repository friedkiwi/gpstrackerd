﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpstrackerd
{
    interface BackendClient
    {
        void HandleTrackingInfoReceived(TrackerMessage message);
    }
}