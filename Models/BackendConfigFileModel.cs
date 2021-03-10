using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpstrackerd.Models
{
    class BackendConfigFileModel
    {
        public string BackendName { get; set; }
        public string BackendEndpoint { get; set; }
        public string AuthToken { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
