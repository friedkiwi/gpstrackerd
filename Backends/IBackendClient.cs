using gpstrackerd.Models;

namespace gpstrackerd.Backends
{
    interface IBackendClient
    {
        void HandleTrackingInfoReceived(TrackerMessage message);
    }
}
