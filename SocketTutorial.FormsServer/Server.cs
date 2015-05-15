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

        public Server()
        {
            InitializeComponent();

            screenWriterDelegate = new AsynchronousSocketListener.ScreenWriterDelegate(WriteToScreen);
            listener = new AsynchronousSocketListener(screenWriterDelegate);
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
            ioThread.Start();
        }
    }
}
