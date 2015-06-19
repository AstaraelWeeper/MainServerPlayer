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
        public int currentFrontDirection = 0; //0 = front, 1 = right, 2 = back, 3 = left
        public int xCoord = 0;
        public int displayNumber;

        public ImageDisplay(int display, string path)
        {
            displayNumber = display;
            InitializeComponent();
            if (display == 1)
            {
                Text = "Image Display 1:Right";
            }
            else if (display == 2)
            {
                Text = "Image Display 2:Left";
            }

            newImage(path);
            picImageDisplay.SizeMode = PictureBoxSizeMode.StretchImage; //don't know if this will be right with the stitched image  
        }

        public void newImage(string path)
        {
            picImageDisplay.Load(path);
        }

        private void ImageDisplay_MouseClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }


    }
}
