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
using System.Net.NetworkInformation;
using InTheHand.Net.Bluetooth;
using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace SocketTutorial.FormsServer
{
    public partial class Server : Form
    {
        public bool wifiOK = false;
        private Thread ioThread;
        private AsynchronousSocketListener listener;
        private BluetoothServer bluetoothListener;
        private AsynchronousSocketListener.ScreenWriterDelegate screenWriterDelegate;
        public delegate string FormActionDelegate(string message);
        private BluetoothServer.ScreenWriterDelegate screenWriterDelegateBT;
       

        public Server()
        {
            InitializeComponent();

            screenWriterDelegate = new AsynchronousSocketListener.ScreenWriterDelegate(WriteToScreen);
            screenWriterDelegateBT = new BluetoothServer.ScreenWriterDelegate(WriteToScreenBT);

            CheckWifi();

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

        public void WriteToScreenBT(string message)
        {
            if (txtServer.InvokeRequired)
            {
                Invoke(screenWriterDelegateBT, message);
            }
            else
            {
                txtServer.Text += "\r\n" + message;
            }
        }

        void CheckWifi()
        {
            Ping myPing = new Ping();
            myPing.PingCompleted += new PingCompletedEventHandler(myPingCompletedCallback);
            try
            {
                myPing.SendAsync("google.com", 3000 /*3 secs timeout*/, new byte[32], new PingOptions(64, true));
 
            }
            catch(Exception e)
            {
                WriteToScreen(e.ToString());
            }
        }

        //check wifi
        public void myPingCompletedCallback(object sender, PingCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                WriteToScreen(e.ToString());

            }

            else if (e.Reply.Status == IPStatus.Success)
            {
               // wifiOk = true;
                //need to callserver load somehow
            }

            else
            {
                bluetoothListener.ServerConnectThread();
            }
            
        }

        private void Server_Load(object sender, EventArgs e)
        {
            
            ioThread = new Thread(new ThreadStart(listener.StartListening));
                ioThread.SetApartmentState(ApartmentState.STA);
                ioThread.Start();
        }


    }
}