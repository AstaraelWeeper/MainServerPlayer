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
using Microsoft.Office.Interop.PowerPoint;

namespace SocketTutorial.FormsServer
{
    public partial class Powerpoint : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hwc, IntPtr hwp);
        public Powerpoint()
        {
            InitializeComponent();
            Process p = Process.Start("C:\\Program Files (x86)\\OpenOffice 4\\program\\soffice.exe"); //example
            Thread.Sleep(500);
            p.WaitForInputIdle();
            SetParent(p.MainWindowHandle,this.Handle);


        }


    }
}
