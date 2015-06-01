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
        private Thread btThread;
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
           // CheckWifi();
            LoadWifiAndBTListeners();
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

        void LoadWifiAndBTListeners()
        {
            listener = new AsynchronousSocketListener(screenWriterDelegate);
                ioThread = new Thread(new ThreadStart(listener.StartListening));
                ioThread.SetApartmentState(ApartmentState.STA);
                ioThread.Start();
                WriteToScreen("Socket server listening");

                bluetoothListener = new BluetoothServer(screenWriterDelegateBT);
                btThread = new Thread(new ThreadStart(bluetoothListener.ServerConnectThread));
                btThread.SetApartmentState(ApartmentState.STA);
                //btThread.Start();
                WriteToScreen("Bluetooth server listening");
        }
        //void CheckWifi()
        //{
        //    Ping myPing = new Ping();

        //    try
        //    {
        //       PingReply replyok = myPing.Send("google.com", 10000 /*3 secs timeout*/, new byte[32], new PingOptions(64, true));


        //         if (replyok.Status == IPStatus.Success)
        //        {
        //            // wifiOk = true;
        //            //need to callserver load somehow
        //            WriteToScreen("WiFi ok. Loading socket server");
        //              listener = new AsynchronousSocketListener(screenWriterDelegate);
        //        ioThread = new Thread(new ThreadStart(listener.StartListening));
        //        ioThread.SetApartmentState(ApartmentState.STA);
        //        ioThread.Start();
        //        }

        //        else
        //        {
        //             WriteToScreen("Wifi offline. Loading Bluetooth server");
        //            bluetoothListener = new BluetoothServer(screenWriterDelegateBT);
        //            ioThread = new Thread(new ThreadStart(bluetoothListener.ServerConnectThread));
        //            ioThread.SetApartmentState(ApartmentState.STA);
        //            ioThread.Start();
        //        }
 
        //    }
        //    catch(Exception e)
        //    {
        //        WriteToScreen(e.ToString());
        //        WriteToScreen("Wifi offline. Loading Bluetooth server");
        //        bluetoothListener = new BluetoothServer(screenWriterDelegateBT);
        //        ioThread = new Thread(new ThreadStart(bluetoothListener.ServerConnectThread));
        //        ioThread.SetApartmentState(ApartmentState.STA);
        //        ioThread.Start();
        //    }
        //}

    }
}