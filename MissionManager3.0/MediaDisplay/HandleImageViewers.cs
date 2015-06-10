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
