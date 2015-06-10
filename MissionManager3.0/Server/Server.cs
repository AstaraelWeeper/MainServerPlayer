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

        public delegate string ImageFormActionDelegate(ImageAction action, string message);
        private ImageFormActionDelegate imageFormActionDelegate;

        HandleVideoPlayers handleVideoPlayers = new HandleVideoPlayers();
        public VideoDisplay videoDisplay = new VideoDisplay();
        public VideoDisplay videoDisplay2 = new VideoDisplay();
        public ImageDisplay imageDisplay;
        public ImageDisplay imageDisplay2;
        HandleImageViewers handleImageViewers = new HandleImageViewers();
        int screens = 2;
        static int resolutionWidth = 1920;
        static int resolutionHight = 1080;


        public Server()
        {
            InitializeComponent();

            screenWriterDelegate = new AsynchronousSocketListener.ScreenWriterDelegate(WriteToScreen);
            screenWriterDelegateBT = new BluetoothServer.ScreenWriterDelegate(WriteToScreenBT);
            videoFormActionDelegate = new VideoFormActionDelegate(PerformVideoAction);
            imageFormActionDelegate = new ImageFormActionDelegate(PerformImageAction);
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
            listener = new AsynchronousSocketListener(screenWriterDelegate, videoFormActionDelegate,imageFormActionDelegate);
            ioThread = new Thread(new ThreadStart(listener.StartListening));
            ioThread.SetApartmentState(ApartmentState.STA);
            ioThread.Start();
            WriteToScreen("Socket server listening");

            bluetoothListener = new BluetoothServer(screenWriterDelegateBT, videoFormActionDelegate, imageFormActionDelegate);
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

        public enum ImageAction
        {
            InitialiseImages,
            ImagePlayerControls
        }
        public string PerformVideoAction(VideoAction action, string message)
        {
            string JsonReturn= "failed at PVA";

            if(txtServer.InvokeRequired)
            {
                return Invoke(videoFormActionDelegate, action, message).ToString();
              
            }

            else
            {
                if (action == VideoAction.InitialisePlayers)
                {
                    JsonReturn = handleVideoPlayers.InitialisePlayers(videoDisplay, videoDisplay2, message );
                }
                else if (action == VideoAction.VideoPlayerControls)
                {
                    JsonReturn = handleVideoPlayers.VideoPlayerControls(message);
                }
                else
                {
                    JsonReturn = "video action failed";
                }

                return JsonReturn;

            }
        }

        public string PerformImageAction(ImageAction action, string message)
        {
            string JsonReturn = "";

            if (txtServer.InvokeRequired)
            {
                Invoke(imageFormActionDelegate, action, message);
                return JsonReturn;
            }

            else
            {
                if (action == ImageAction.InitialiseImages)
                {
                    imageDisplay = new ImageDisplay(1, message,resolutionWidth); //expecting direction to reset to 0 if new one opened
                    imageDisplay2 = new ImageDisplay(2, message, resolutionWidth);

                    imageDisplay.Width = screens * resolutionWidth;
                    imageDisplay2.Width = 0;
                    imageDisplay.Height = resolutionHight;
                    imageDisplay2.Height = resolutionHight;
                    imageDisplay.Location = imageDisplay.imageViewerLocation;
                    imageDisplay2.Location = imageDisplay2.imageViewerLocation;
                    imageDisplay.Show();
                    imageDisplay2.Show();

                    JsonReturn = "{\"messageType\":\"ImageViewer\",\"messageBody\":\"Image Initialised\"}";
                    handleVideoPlayers.videoDisplay.MinimiseForm();
                    handleVideoPlayers.videoDisplay2.MinimiseForm();
                    return JsonReturn;

                }

                else
                {
                    JsonReturn = "image action failed";
                    return JsonReturn;
                }

            }
        }

    }
}