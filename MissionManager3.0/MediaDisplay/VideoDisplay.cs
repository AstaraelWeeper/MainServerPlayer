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
    }
  
}
