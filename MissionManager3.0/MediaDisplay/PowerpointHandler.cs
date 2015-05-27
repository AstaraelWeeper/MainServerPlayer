using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketTutorial.FormsServer
{
    class PowerpointHandler
    {
        string JsonReturn;
        private Powerpoint powerpointForm = null;

        public string initialisePowerpoint(string path)
        {
            Powerpoint powerpointForm = new Powerpoint();//need to pass in file to open and figure out how to control powerpoint
            JsonReturn = "{\"messageType\":\"OpenPowerpoint\",\"messageBody\":\"Opened\"}";
            return JsonReturn;
        }

        public void closePowerpoint()
        {
            powerpointForm.Close();
        }
    }
}
