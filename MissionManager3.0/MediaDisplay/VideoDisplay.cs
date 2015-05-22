using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shell32;
using System.IO;
using AxAXVLC;

namespace SocketTutorial.FormsServer
{
    public partial class VideoDisplay : Form
    {
        string path;
        public VideoDisplay(string pathIn)
        {
            path = pathIn;
            InitializeComponent();
           
            axVLCPlugin21.playlist.add("File:///"+path,null, null);
            axVLCPlugin21.playlist.playItem(0);
            
        }
        public void IncreaseVolume()
        {
          //  axVLCPlugin21.Volume. 
        }
        public void DecreaseVolume()
        {
           // axWindowsMediaPlayer1.settings.volume--;
        }

        public void sendVideoLength()
        {
            //initially. can it be found for the initial message?
            TimeSpan duration;

            if (GetDuration(path, out duration))
            {
                //use the returned time to send
            }
        }

        public void updateVideoTime()
        {
           //need to send like this regularly { "messageType": "VideoPlayer", "messageBody" : "move-00:04:48" }
        }

        public static bool GetDuration(string filename, out TimeSpan duration)
        {
            try
            {
                var shl = new Shell();
                var fldr = shl.NameSpace(Path.GetDirectoryName(filename));
                var itm = fldr.ParseName(Path.GetFileName(filename));

                // Index 27 is the video duration [This may not always be the case]
                var propValue = fldr.GetDetailsOf(itm, 27);

                return TimeSpan.TryParse(propValue, out duration);
            }
            catch (Exception)
            {
                duration = new TimeSpan();
                return false;
            }
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {

        }
    }
  
}
