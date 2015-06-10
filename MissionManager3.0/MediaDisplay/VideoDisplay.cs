﻿using System;
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
        string JsonReturn;
        static int resolutionWidth;
        public int currentFrontDirection = 0; //0 = front, 1 = right, 2 = back, 3 = left
        static int picX;
        static int y = 0;
        public Point videoDisplayLocation;

        static string returnPlugin = "";
        string returnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"" + returnPlugin + "\"}";

        public VideoDisplay(int display, string path, int resWidth)
        {
            InitializeComponent();
            if (display == 1)
            {
                picX = (currentFrontDirection + 1) * resolutionWidth;
                videoDisplayLocation = new Point(picX, y);
            }
            else if (display == 2)
            {
                picX = (currentFrontDirection) * resolutionWidth;
                videoDisplayLocation = new Point(picX, y);
            }
            resolutionWidth = resWidth;

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
        public string rotateRight()
        {
            string stringReturnMessage;

            if (currentFrontDirection < 3)
            {
                currentFrontDirection++;
            }
            else if (currentFrontDirection == 3)
            {
                currentFrontDirection = 0;
            }
            this.Location = videoDisplayLocation;

            stringReturnMessage = getFacingDirectionJSON();
            return stringReturnMessage;

        }

        public string rotateLeft()
        {
            string stringReturnMessage;

            if (currentFrontDirection > 0)
            {
                currentFrontDirection--;
            }
            else if (currentFrontDirection == 0)
            {
                currentFrontDirection = 3;
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
