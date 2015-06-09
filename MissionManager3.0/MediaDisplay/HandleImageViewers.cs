using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SocketTutorial.FormsServer
{
    class HandleImageViewers
    {

        string JsonReturn;
        int screens = 2;
        static int resolutionWidth = 1920;
        static int resolutionHight = 1080;
        static int currentFrontDirection = 0; //0 = front, 1 = right, 2 = back, 3 = left
        static int pic1X = (currentFrontDirection + 1) * resolutionWidth;
        static int pic2X = (currentFrontDirection) * resolutionWidth;
        static int y = 0;
        Point imageViewer1Location = new Point(pic1X, y);
        Point imageViewer2Location = new Point(pic2X, y);
       public ImageDisplay imageDisplay = new ImageDisplay();
       public ImageDisplay imageDisplay2 = new ImageDisplay();

        public string InitialiseViewers(ImageDisplay _imageDisplay, ImageDisplay _imageDisplay2, string path)
        {
            imageDisplay = _imageDisplay;
            imageDisplay2 = _imageDisplay2;

            imageDisplay.Height = resolutionHight;
            imageDisplay2.Height = resolutionHight;
            imageDisplay.Width = screens * resolutionWidth;
            imageDisplay2.Width = 0;

            imageDisplay2.StartPosition = FormStartPosition.Manual;
            imageDisplay2.Location = imageViewer2Location;

            imageDisplay.InitialiseImage(path);
            imageDisplay2.InitialiseImage(path);
            imageDisplay2.Show();
            imageDisplay.Show();
            

            JsonReturn = "{\"messageType\":\"ImageViewer\",\"messageBody\":\"Image Initialised\"}";
            return JsonReturn;
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
            imageDisplay.Location = imageViewer1Location;
            imageDisplay2.Location = imageViewer2Location;

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
            imageDisplay.Location = imageViewer1Location;
            imageDisplay2.Location = imageViewer2Location;

            stringReturnMessage = getFacingDirectionJSON();
            return stringReturnMessage;
        }

        private string getFacingDirectionJSON()
        {
            string stringReturnMessage;
            if (currentFrontDirection == 0)
            {
                stringReturnMessage = "{\"messageType\":\"ImagePlayer\",\"messageBody\":\"Facing Front\"}";
                return stringReturnMessage;
            }
            else if (currentFrontDirection == 1)
            {
                stringReturnMessage = "{\"messageType\":\"ImagePlayer\",\"messageBody\":\"Facing Right\"}";
                return stringReturnMessage;
            }
            else if (currentFrontDirection == 2)
            {
                stringReturnMessage = "{\"messageType\":\"ImagePlayer\",\"messageBody\":\"Facing Back\"}";
                return stringReturnMessage;
            }
            else if (currentFrontDirection == 3)
            {
                stringReturnMessage = "{\"messageType\":\"ImagePlayer\",\"messageBody\":\"Facing Left\"}";
                return stringReturnMessage;
            }

            else
            {
                stringReturnMessage = "{\"messageType\":\"ImagePlayer\",\"messageBody\":\"Facing Calculation Failure\"}";
                return stringReturnMessage;
            }
        }

        public string ImageViewerControls(string message)
        {
            if (imageDisplay != null)
            {
                if (message.Contains("rotate right"))
                {
                    JsonReturn =  rotateRight();
                     
                    return JsonReturn;
                }
                else if (message.Contains("rotate left"))
                {
                    JsonReturn = rotateLeft();
                    return JsonReturn;
                }
                else
                {
                    JsonReturn = "{\"messageType\":\"ImageViewer\",\"messageBody\":\"RequestFailed\"}";
                    return JsonReturn;
                }
            }
            else
            {
                JsonReturn= "{\"messageType\":\"ImageViewer\",\"messageBody\":\"No Image Viewer\"}";
                return JsonReturn;
            }
        }
    }
}
