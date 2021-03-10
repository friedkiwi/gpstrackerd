using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpstrackerd.Models
{
    class ConfigFileModel
    {
        public int Port { get; set; }
        public List<BackendConfigFileModel> Backends { get; set; } = new List<BackendConfigFileModel>();
    }
}
