using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketTutorial.FormsServer.MediaDisplay
{
    public class VideoSettings
    {
        public int volume { get; set; }
        public int playbackSpeed { get; set; }

        public VideoSettings(string path)
        {
            //give initial values
        }

    }
}
