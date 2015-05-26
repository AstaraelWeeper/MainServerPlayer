using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SocketTutorial.FormsServer
{
    class HandleVideoPlayers
    {
        private VideoDisplay videoDisplay = null;
        private VideoDisplay videoDisplay2 = null;
        int screens = 2;
       static  int resolutionWidth = 1920;
        static int resolutionHight = 1080;
        static int currentFrontDirection = 0; //0 = front, 1 = right, 2 = back, 3 = left
        static int vid1X = (currentFrontDirection + 1) * resolutionWidth;
        static int vid2X = (currentFrontDirection) * resolutionWidth;
        static int y = 0;
        Point videoPlayer1Location = new Point(vid1X,y);
        Point videoPlayer2Location = new Point(vid2X,y);
        

        public void initialisePlayers(string path)
        {
            videoDisplay = new VideoDisplay(path);
                        videoDisplay2 = new VideoDisplay(path);
                        videoDisplay.Height = resolutionHight;
                        videoDisplay2.Height = resolutionHight;
                        videoDisplay.Width = screens * resolutionWidth;
                        videoDisplay2.Width = 0;

                        videoDisplay2.StartPosition = FormStartPosition.Manual;
                        videoDisplay2.Location = videoPlayer2Location;
                        videoDisplay.Show();
                        videoDisplay2.Show();

        }

        private string rotateRight()
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
            videoDisplay.Location = videoPlayer1Location;
            videoDisplay2.Location = videoPlayer2Location;

            stringReturnMessage = getFacingDirectionJSON();
            return stringReturnMessage;
            
        }

        private string rotateLeft()
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
            videoDisplay.Location = videoPlayer1Location;
            videoDisplay2.Location = videoPlayer2Location;

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

        public string GetVideoLength()
        {
            string stringReturnMessage = videoDisplay.getVideoLength();
            return stringReturnMessage;
        }
        public string VideoPlayerControls(string message)
        {
            string stringReturnMessage = "";

            if (videoDisplay != null)
            {
                if (message.Contains("stopping"))
                {
                    stringReturnMessage = videoDisplay.Stop();
                    videoDisplay2.Stop();
                    return stringReturnMessage;
                }
                else if (message.Contains("pausing"))
                {
                    stringReturnMessage = videoDisplay.Pause();
                    videoDisplay2.Pause();
                    return stringReturnMessage;
                }
                else if (message.Contains("playing"))
                {
                    stringReturnMessage = videoDisplay.Play();
                    videoDisplay2.Play();
                    return stringReturnMessage;
                }

                else if (message.Contains("rotate right"))
                {
                    stringReturnMessage =  rotateRight();
                     
                    return stringReturnMessage;
                }
                else if (message.Contains("rotate left"))
                {
                    rotateLeft();
                    return stringReturnMessage;
                }

                else if (message.Contains("Raising Volume"))
                {
                        stringReturnMessage = videoDisplay.IncreaseVolume();
                        return stringReturnMessage;
                }
                else if (message.Contains("Lowering Volume"))
                {
                        stringReturnMessage = videoDisplay.DecreaseVolume();
                        return stringReturnMessage;
                }

                else
                {
                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"RequestFailed\"}";
                    return stringReturnMessage;
                }
            }
            else
            {
                stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                return stringReturnMessage;
            }

        } //end of method
    }
}
