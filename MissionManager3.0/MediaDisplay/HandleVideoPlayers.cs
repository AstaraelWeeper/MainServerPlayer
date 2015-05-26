using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SocketTutorial.FormsServer
{
    class HandleVideoPlayers
    {
        private VideoDisplay videoDisplay = null;
        private VideoDisplay videoDisplay2 = null;
        int screens = 2;
        int resolutionWidth = 1920;
        int resolutionHight = 1080;
        int currentFrontDirection = 1; //1 = front, 2 = right, 3 = back, 4 = left

        public void initialisePlayers(string path)
        {
            videoDisplay = new VideoDisplay(path);
                        videoDisplay2 = new VideoDisplay(path);
                        videoDisplay.Height = resolutionHight;
                        videoDisplay2.Height = resolutionHight;
                        videoDisplay.Width = screens * resolutionWidth;
                        videoDisplay2.Width = 0;

                        videoDisplay2.StartPosition = FormStartPosition.Manual;
                      //  videoDisplay2 = videoDisplay.Width;
                        videoDisplay.Show();
                        videoDisplay2.Show();

        }

        private void rotateRight()
        {
        }

        private void rotateLeft()
        {
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
