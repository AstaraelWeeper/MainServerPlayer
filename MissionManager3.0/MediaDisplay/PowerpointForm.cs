using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace SocketTutorial.FormsServer
{
    public partial class PowerpointForm : Form
    {
        PowerPoint.Presentation file; 
        int slideIndex = 0;

        public PowerpointForm(string path)
        {
            InitializeComponent();
            var app = new PowerPoint.Application();

            var pres = app.Presentations;

            file = pres.Open(path, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);

        }

        public string NextSlide()
        {
            string JsonReturn;
            slideIndex++;
            file.SlideShowWindow.View.GotoSlide(slideIndex, MsoTriState.msoFalse);
            JsonReturn = "{\"messageType\":\"Powerpoint\",\"messageBody\":\"NextSlide\"}";
            return JsonReturn;
        }

        public string PreviousSlide()
        {
            string JsonReturn;
            slideIndex--;
            file.SlideShowWindow.View.GotoSlide(slideIndex, MsoTriState.msoFalse);
            JsonReturn = "{\"messageType\":\"Powerpoint\",\"messageBody\":\"PreviousSlide\"}";
            return JsonReturn;
        }

        public string GotoSlide(int slide)
        {
            string JsonReturn;
            slideIndex = slide;
            file.SlideShowWindow.View.GotoSlide(slideIndex, MsoTriState.msoFalse);
            JsonReturn = "{\"messageType\":\"Powerpoint\",\"messageBody\":\"OnSelectedSlide\"}";
            return JsonReturn;
        }
    }
}
