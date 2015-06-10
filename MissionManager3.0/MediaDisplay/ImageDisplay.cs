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
    public partial class ImageDisplay : Form
    {
        string JsonReturn;
        static int resolutionWidth; 
        public int currentFrontDirection = 0; //0 = front, 1 = right, 2 = back, 3 = left
        static int picX;
        static int y = 0;
        public Point imageViewerLocation;

        public ImageDisplay(int display, string path, int resWidth)
        {
            InitializeComponent();
            if (display == 1)
            {
                picX = (currentFrontDirection + 1) * resolutionWidth;
                imageViewerLocation = new Point(picX, y);
            }
            else if (display == 2)
            {
                picX = (currentFrontDirection) * resolutionWidth;
                imageViewerLocation = new Point(picX, y);
            }
            resolutionWidth = resWidth;
            picImageDisplay.Load(path);
            picImageDisplay.SizeMode = PictureBoxSizeMode.StretchImage; //don't know if this will be right with the stitched image  

        }
        public void rotateRight()
        {
            if (currentFrontDirection < 3)
            {
                currentFrontDirection++;
            }
            else if (currentFrontDirection == 3)
            {
                currentFrontDirection = 0;
            }

        }
        public void rotateLeft()
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
    }
}
