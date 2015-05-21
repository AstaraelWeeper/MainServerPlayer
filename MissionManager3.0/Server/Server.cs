using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SocketTutorial.FormsServer
{
    public partial class Server : Form
    {
        
        private Thread ioThread;
        private AsynchronousSocketListener listener;
        private AsynchronousSocketListener.ScreenWriterDelegate screenWriterDelegate;
        public delegate void FormActionDelegate(string message);
         
        public Server()
        {
            InitializeComponent();

            screenWriterDelegate = new AsynchronousSocketListener.ScreenWriterDelegate(WriteToScreen);
            var openNewFormDelegate = new FormActionDelegate(OpenNewFormAction);
            listener = new AsynchronousSocketListener(screenWriterDelegate,openNewFormDelegate);
                       
        }

        public void WriteToScreen(string message)
        {
            if (txtServer.InvokeRequired)
            {
                Invoke(screenWriterDelegate, message);
            }
            else
            {
                txtServer.Text += "\r\n" + message;
            }
        }

        
        private void Server_Load(object sender, EventArgs e)
        {
            ioThread = new Thread(new ThreadStart(listener.StartListening));
            ioThread.SetApartmentState(ApartmentState.STA);
            ioThread.Start();
        }

        [STAThread]
        private void OpenNewFormAction(string message)
        {
            if (message.Contains("Launching Video"))
            {
                string intro = "{\"messageType\":\"LaunchVideo\"\"messageBody:\"\"Launching Video\"";
                string path = intro.Remove(0, intro.Length);
                
                //VideoDisplay videoDisplay = new VideoDisplay(path);
            }
            else if (message.Contains("Launching Image"))
            {
            }
            else if (message == "Raising Volume" || message == "Lowering Volume")
            {
            }
            else if (message == "Restarting System" || message == "Restarting Mission Manager")
            {
            }

        }

        
 
    }
}
