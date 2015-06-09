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
        private BluetoothServer.ScreenWriterDelegate screenWriterDelegateBT;
        public delegate string VideoFormActionDelegate(VideoAction action, string message);
        private VideoFormActionDelegate videoFormActionDelegate;

        HandleVideoPlayers handleVideoPlayers = new HandleVideoPlayers();
        public VideoDisplay videoDisplay = new VideoDisplay();
        public VideoDisplay videoDisplay2 = new VideoDisplay();


        public Server()
        {
            InitializeComponent();

            screenWriterDelegate = new AsynchronousSocketListener.ScreenWriterDelegate(WriteToScreen);
            screenWriterDelegateBT = new BluetoothServer.ScreenWriterDelegate(WriteToScreenBT);
            videoFormActionDelegate = new VideoFormActionDelegate(PerformVideoAction);
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
            listener = new AsynchronousSocketListener(screenWriterDelegate, videoFormActionDelegate);
            ioThread = new Thread(new ThreadStart(listener.StartListening));
            ioThread.SetApartmentState(ApartmentState.STA);
            ioThread.Start();
            WriteToScreen("Socket server listening");

            bluetoothListener = new BluetoothServer(screenWriterDelegateBT, videoFormActionDelegate);
            btThread = new Thread(new ThreadStart(bluetoothListener.ServerConnectThread));
            btThread.SetApartmentState(ApartmentState.STA);
            btThread.Start();
            WriteToScreen("Bluetooth server listening");
        }

        public enum VideoAction
        {
            InitialisePlayers,
            VideoPlayerControls
        }
        public string PerformVideoAction(VideoAction action, string message)
        {
            string JsonReturn= "";

            if(txtServer.InvokeRequired)
            {
                Invoke(videoFormActionDelegate, action, message);
                return JsonReturn;
            }

            else
            {
                if (action == VideoAction.InitialisePlayers)
                {
                    JsonReturn = handleVideoPlayers.InitialisePlayers(videoDisplay, videoDisplay2, message );
                    return JsonReturn;

                }
                else if (action == VideoAction.VideoPlayerControls)
                {
                    JsonReturn = handleVideoPlayers.VideoPlayerControls(message);
                    return JsonReturn;

                }
                else
                {
                    JsonReturn = "video action failed";
                    return JsonReturn;
                }

            }
        }

    }
}