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
        public List<string> File = new List<string>();
       public string name { get; set; }
        public string path { get; set; }
       public  string ext { get; set; }

    }
}
