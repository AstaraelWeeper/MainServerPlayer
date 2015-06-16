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
        static int resolutionWidth;
        public int currentFrontDirection = 0; //0 = front, 1 = right, 2 = back, 3 = left
        static int picX;
        static int y = 0;
        public Point videoDisplayLocation;
        int displayNumber;

        static string returnPlugin = "";
        string returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";

        public VideoDisplay(int display, string path, int resWidth)
        {
            resolutionWidth = resWidth;
            displayNumber = display;
            InitializeComponent();
            if (display == 1)
            {
                
                picX = currentFrontDirection * resolutionWidth; //start at 0,0
                videoDisplayLocation = new Point(picX, y);
            }
            else if (display == 2)
            {
                picX = (currentFrontDirection-3) * resolutionWidth; //start at -3,0
                videoDisplayLocation = new Point(picX, y);
                axVLCPlugin21.Volume = 0;
            }
            

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

        public string updateVideoTime()
        {
           //need to send like this regularly { "messageType": "VideoPlayerSync", "messageBody" : "00:04:48" }
            var currentPlayDuration = TimeSpan.FromMilliseconds(axVLCPlugin21.input.Time);
            string jsonReturn = "{\"messageType\":\"VideoPlayerSync\",\"messageBody\":" + currentPlayDuration.Hours.ToString() + currentPlayDuration.Minutes.ToString() + currentPlayDuration.Seconds.ToString() + "\"}";
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
        public string rotateRight()
        {
            string stringReturnMessage;
            if (displayNumber == 1)
            {
                if (currentFrontDirection < 3) //0 - 3 range
                {
                    currentFrontDirection++;
                }
                else if (currentFrontDirection == 3)
                {
                    currentFrontDirection = 0;
                }
            }
            else if (displayNumber == 2)
            {
                if (currentFrontDirection < -1) //-4 to -1 range
                {
                    currentFrontDirection++;
                }
                else if (currentFrontDirection == -1)
                {
                    currentFrontDirection = -4;
                }
            }
            this.Location = videoDisplayLocation;

            stringReturnMessage = getFacingDirectionJSON();
            return stringReturnMessage;

        }

        public string rotateLeft()
        {
            string stringReturnMessage;
            if (displayNumber == 1)
            {
                if (currentFrontDirection > 0)
                {
                    currentFrontDirection--;
                }
                else if (currentFrontDirection == 0)
                {
                    currentFrontDirection = 3;
                }
            }
            else if (displayNumber == 2)
            {
                if (currentFrontDirection > -4)
                {
                    currentFrontDirection--;
                }
                else if (currentFrontDirection == -4)
                {
                    currentFrontDirection = -1;
                }
            }
           this.Location = videoDisplayLocation;

            stringReturnMessage = getFacingDirectionJSON();
            return stringReturnMessage;
        }

        private string getFacingDirectionJSON()
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
    }
  
}
