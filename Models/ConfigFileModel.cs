using System.Collections.Generic;

namespace gpstrackerd.Models
{
    class ConfigFileModel
    {
        public int Port { get; set; }
        public List<BackendConfigFileModel> Backends { get; set; } = new List<BackendConfigFileModel>();
    }
}
