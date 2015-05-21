using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SocketTutorial.FormsServer
{
    public partial class VideoDisplay : Form
    {
        public VideoDisplay(string path)
        {
            InitializeComponent();
            axWindowsMediaPlayer1.URL = path;
        }
        public void IncreaseVolume()
        {
            axWindowsMediaPlayer1.settings.volume++;
        }
        public void DecreaseVolume()
        {
            axWindowsMediaPlayer1.settings.volume--;
        }

        public void changeVideo(string newPath)
        {
            axWindowsMediaPlayer1.URL = newPath;
        }

        public void sendVideoLength()
        {
            //initially. can it be found for the initial message?
        }

        public void updateVideoTime()
        {
           //need to send like this regularly { "messageType": "VideoPlayer", "messageBody" : "move-00:04:48" }
        }
    }
  
}
