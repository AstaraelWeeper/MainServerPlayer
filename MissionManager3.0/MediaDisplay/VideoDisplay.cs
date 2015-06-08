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
        static string returnPlugin = "";
        string returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";

        public VideoDisplay()
        {
            InitializeComponent();
        }

        public void VideoDisplayInitialise(string pathIn)
        {
            path = pathIn;
            InitializeComponent();
           
            axVLCPlugin21.playlist.add("File:///"+path,null, null);
            axVLCPlugin21.playlist.playItem(0);
            
        }
        public string Pause()
        {
                axVLCPlugin21.playlist.pause();
                returnPlugin = "Paused";
                returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";
                return returnMessage;
        }

        public string Stop()
        {
            axVLCPlugin21.playlist.stop();
            returnPlugin = "Stopped";
            returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";
            return returnMessage;
        }

        public string Play()
        {
            axVLCPlugin21.playlist.play();
            returnPlugin = "Playing";
            returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";
            return returnMessage;
        }

        public string IncreaseVolume()
        {
            axVLCPlugin21.Volume++; //check if it needs a larger increment
            returnPlugin = "Volume Increased";
            returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";
            return returnMessage;
        }
        public string DecreaseVolume()
        {
            axVLCPlugin21.Volume--;
            returnPlugin = "Volume Decreased";
            returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";
            return returnMessage;
        }

        public string getVideoLength(string path)
        {
            
            TimeSpan duration;

            if (GetDuration(path, out duration))
            {
                //use the returned time to send
                returnPlugin = duration.ToString();
                returnMessage = "{\"messageType\":\"LaunchVideo\",\"messageBody\":\"" + returnPlugin + "\"}"; 
                return returnMessage;
            }
            else
            {
                returnPlugin = "GetDuration Failed";
                returnMessage = "{\"messageType\":\"LaunchVideo\",\"messageBody\":\"" + returnPlugin + "\"}"; 
                return returnMessage;
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

    }
  
}
