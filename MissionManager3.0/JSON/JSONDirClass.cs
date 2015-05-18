using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SocketTutorial.FormsServer
{
    class JSONDirClass
    {
        [JsonProperty("elr")]
        public string elr { get; set; }
    }
}
