using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SocketTutorial.FormsServer
{
    public partial class Server : Form
    {
        List<string> History = new List<string>();

        private Thread ioThread;
        private AsynchronousSocketListener listener;
        private AsynchronousSocketListener.ScreenWriterDelegate screenWriterDelegate;
        public delegate void FormActionDelegate(string message);
        private FormActionDelegate openNewFormDelegate;
        private VideoDisplay videoDisplay = null;
        private ImageDisplay imageDisplay = null;

        public Server()
        {
            InitializeComponent();

            screenWriterDelegate = new AsynchronousSocketListener.ScreenWriterDelegate(WriteToScreen);
            openNewFormDelegate = new FormActionDelegate(OpenNewFormAction);
            listener = new AsynchronousSocketListener(screenWriterDelegate, openNewFormDelegate);

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
            string intro = "{\"messageType\":\"LaunchVideo\"\"messageBody:\"\"Launching Video\"";
            string path = message.Remove(0, intro.Length+1);
            path = path.Remove((path.Length - 2), 2);
            if (message.Contains("Launching Video"))
            {
                if (this.InvokeRequired)
                {
                    Invoke(openNewFormDelegate, message); //having issues with threading
                }
                else
                {
                    History.Add(path);

                    if (videoDisplay == null)
                    {
                        videoDisplay = new VideoDisplay(path);
                        videoDisplay.Show();
                    }
                    else
                    {
                        videoDisplay.Close();
                        videoDisplay = new VideoDisplay(path);
                        videoDisplay.Show();
                    }
                }

            }
            else if (message.Contains("Launching Image"))
            {
                if (this.InvokeRequired)
                {
                    Invoke(openNewFormDelegate, message); //having issues with threading
                }
                else
                {
                    History.Add(path);

                    if (imageDisplay == null)
                    {
                        imageDisplay = new ImageDisplay(path);
                        imageDisplay.Show();
                    }
                    else
                    {
                        imageDisplay.Close();
                        imageDisplay.Show();
                    }
                }
            }
            else if (message.Contains("Raising Volume")) //build it for media player atm
            {
                if (videoDisplay != null)
                {
                    videoDisplay.IncreaseVolume();
                }
            }
            else if (message.Contains("Lowering Volume"))
            {
                if (videoDisplay != null)
                {
                    videoDisplay.DecreaseVolume();
                }
            }
            else if (message.Contains("Restarting System"))
            {
                Process.Start("shutdown", "/r /t 0");
            }
            else if (message.Contains("Shutting Down System"))
            {
                Process.Start("shutdown", "s /t 0");
            }
            else if (message.Contains("Restarting Mission Manager"))
            {
                System.Diagnostics.Process.Start(Application.ExecutablePath); // to start new instance of application
                this.Close(); //to turn off current app
            }

        }



    }
}