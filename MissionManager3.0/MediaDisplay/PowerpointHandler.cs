using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketTutorial.FormsServer
{
    class PowerpointHandler
    {
        string JsonReturn;
        private PowerpointForm powerpointForm = null;

        public string initialisePowerpoint(string path)
        {
            PowerpointForm powerpointForm = new PowerpointForm(path);//need to pass in file to open and figure out how to control powerpoint
            JsonReturn = "{\"messageType\":\"OpenPowerpoint\",\"messageBody\":\"Opened\"}";
            return JsonReturn;
        }

        public string NextSlide()
        {
            JsonReturn = powerpointForm.NextSlide();
            return JsonReturn;
        }

        public string PreviousSlide()
        {
            JsonReturn = powerpointForm.PreviousSlide();
            return JsonReturn;
        }

        public void closePowerpoint()
        {
            powerpointForm.Close();
        }

        public string GotoSlide(int slide)
        {
            JsonReturn = powerpointForm.GotoSlide(slide);
            return JsonReturn;
        }
    }
}
