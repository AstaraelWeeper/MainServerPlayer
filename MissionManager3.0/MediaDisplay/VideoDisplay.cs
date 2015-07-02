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
        public int currentFrontDirection = 0; //0 = front, 1 = right, 2 = back, 3 = left
        public int xCoord = 0;
        public int displayNumber;
        TimeSpan duration;

        static string returnPlugin = "";
        string returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";

        public VideoDisplay(int display, string path)
        {
            displayNumber = display;
            InitializeComponent();
            if (display == 1)
            {
                Text = "Video Display 1:Right";
            }
            else if (display == 2)
            {
                axVLCPlugin21.Volume = 0;
                Text = "Video Display 2:Left";
            }

            newVideo(path);

        }

        public void newVideo(string path)
        {
            axVLCPlugin21.playlist.items.clear();
            axVLCPlugin21.playlist.add("File:///" + path, null, null);
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
            axVLCPlugin21.Volume += 10; //check if it needs a larger increment
            returnPlugin = "Volume Increased";
            returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";
            return returnMessage;
        }
        public string DecreaseVolume()
        {
            axVLCPlugin21.Volume -= 10;
            returnPlugin = "Volume Decreased";
            returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";
            return returnMessage;
        }

        public string getVideoLength(string path)
        {
            

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

        public string SyncVideoTime()
        {
           //need to send like this regularly { "messageType": "VideoPlayerSync", "messageBody" : "00:04:48" }
            var currentPlayDuration = TimeSpan.FromMilliseconds(axVLCPlugin21.input.Time);
            string hours = currentPlayDuration.Hours.ToString();
            string minutes = currentPlayDuration.Minutes.ToString();
            int secondsInt = currentPlayDuration.Seconds;
            if(currentPlayDuration.Milliseconds > 500)
            {
                secondsInt++;
            }
            string seconds = secondsInt.ToString();
            if (hours.Length == 1)
            {
                hours = hours.PadLeft(2,'0');
            }
            if (minutes.Length == 1)
            {
                minutes = minutes.PadLeft(2,'0');
            }
            if (seconds.Length == 1)
            {
                seconds = seconds.PadLeft(2,'0');
            }
            string jsonReturn = "{\"messageType\":\"VideoPlayerSync\",\"messageBody\":\"" + hours + ":" + minutes +":" + seconds + "\"}";
            return jsonReturn;
        }

        public string UpdateVideoTime(TimeSpan newTime) //take in timespan
        {
            double changeMilliSeconds = newTime.TotalMilliseconds;
            axVLCPlugin21.input.Time = changeMilliSeconds;
            string jsonReturn = SyncVideoTime();
            return jsonReturn;
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


        public string Loop(string willLoop)
        {
            string stringReturnMessage = "";

            if(willLoop.Contains("off"))
            {
                axVLCPlugin21.AutoLoop = false;
                stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Loop off\"}";
            }
            else if (willLoop.Contains("on"))
            {
                axVLCPlugin21.AutoLoop = true;
                stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Loop on\"}";
            }

            return stringReturnMessage;
        }

        public string getFacingDirectionJSON()
        {
            string stringReturnMessage;
            if (currentFrontDirection == 0)
            {
                stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Facing Front\"}";
                return stringReturnMessage;
            }
            else if (currentFrontDirection == 1)
            {
                stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Facing Right\"}";
                return stringReturnMessage;
            }
            else if (currentFrontDirection == 2)
            {
                stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Facing Back\"}";
                return stringReturnMessage;
            }
            else if (currentFrontDirection == 3)
            {
                stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Facing Left\"}";
                return stringReturnMessage;
            }

            else
            {
                stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Facing Calculation Failure\"}";
                return stringReturnMessage;
            }
        }


        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void VideoDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

    }
  
}
