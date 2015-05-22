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
        public ImageDisplay(string path)
        {
            InitializeComponent();
            picImageDisplay.SizeMode = PictureBoxSizeMode.StretchImage; //don't know if this will be right with the stitched image
            picImageDisplay.Load(path);
        }
    }
}
