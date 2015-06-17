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

        public VideoDisplay videoDisplay = null;
        public VideoDisplay videoDisplay2 = null;
        public ImageDisplay imageDisplay = null;
        public ImageDisplay imageDisplay2 = null;

        int screens = 4; //should be 4 in live no matter how many screens
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
                    if(videoDisplay !=null)
                    {                       
                        videoDisplay.newVideo(message);
                        videoDisplay2.newVideo(message);
                    }
                    else
                    {
                        videoDisplay = new VideoDisplay(1, message, resolutionWidth, screens); //expecting direction to reset to 0 if new one opened
                        videoDisplay2 = new VideoDisplay(2, message, resolutionWidth, screens);

                    }

                    videoDisplay.Width = screens * resolutionWidth;
                    videoDisplay2.Width = screens * resolutionWidth;
                    videoDisplay.Height = resolutionHight;
                    videoDisplay2.Height = resolutionHight;
                    videoDisplay.Location = videoDisplay.videoDisplayLocation;
                    videoDisplay2.Location = videoDisplay2.videoDisplayLocation;
                    
                    //videoDisplay.WindowState = FormWindowState.Maximized;
                    //videoDisplay2.WindowState = FormWindowState.Maximized;
                    //imageDisplay.WindowState = FormWindowState.Minimized;
                    //imageDisplay2.WindowState = FormWindowState.Minimized;
                    videoDisplay2.Show();
                    videoDisplay.Show();

                    JsonReturn = videoDisplay.getVideoLength(message);
                    return JsonReturn;
                }
                else if (action == VideoAction.VideoPlayerControls)
                {
                    JsonReturn = VideoPlayerControls(message);
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
                    imageDisplay2.Width = screens * resolutionWidth;
                    imageDisplay.Height = resolutionHight;
                    imageDisplay2.Height = resolutionHight;
                    imageDisplay.Location = imageDisplay.imageViewerLocation;
                    imageDisplay2.Location = imageDisplay2.imageViewerLocation;
                    //imageDisplay.WindowState = FormWindowState.Maximized;
                    //imageDisplay2.WindowState = FormWindowState.Maximized;
                    //videoDisplay.WindowState = FormWindowState.Minimized;
                    //videoDisplay2.WindowState = FormWindowState.Minimized;
                    imageDisplay.Show();
                    imageDisplay2.Show();

                    JsonReturn = "{\"messageType\":\"ImageViewer\",\"messageBody\":\"Image Initialised\"}";
                    
                    return JsonReturn;

                }

                else
                {
                    JsonReturn = "image action failed";
                    return JsonReturn;
                }

            }
        }
        public string VideoPlayerControls(string message)
        {
            string stringReturnMessage = "";

            if (videoDisplay != null)
            {
                if (message.Contains("stop"))
                {
                    stringReturnMessage = videoDisplay.Stop();
                    videoDisplay2.Stop();
                    return stringReturnMessage;
                }
                else if (message.Contains("pause"))
                {
                    stringReturnMessage = videoDisplay.Pause();
                    videoDisplay2.Pause();
                    return stringReturnMessage;
                }
                else if (message.Contains("play"))
                {
                    stringReturnMessage = videoDisplay.Play();
                    videoDisplay2.Play();
                    return stringReturnMessage;
                }

                else if (message.Contains("rotate right"))
                {
                    videoDisplay2.rotateRight();
                    stringReturnMessage = videoDisplay.rotateRight();
                    videoDisplay.Location = videoDisplay.videoDisplayLocation;
                    videoDisplay2.Location = videoDisplay2.Location;
                    videoDisplay.Focus();
                    videoDisplay2.Focus();
                    return stringReturnMessage;
                }
                else if (message.Contains("rotate left"))
                {
                    videoDisplay2.rotateLeft();
                    stringReturnMessage = videoDisplay.rotateLeft();
                    videoDisplay.Location = videoDisplay.videoDisplayLocation;
                    videoDisplay2.Location = videoDisplay2.Location;
                    return stringReturnMessage;
                }

                else if (message.Contains("Raise Volume"))
                {
                    stringReturnMessage = videoDisplay.IncreaseVolume();
                    return stringReturnMessage;
                }
                else if (message.Contains("Lower Volume"))
                {
                    stringReturnMessage = videoDisplay.DecreaseVolume();
                    return stringReturnMessage;
                }
                else if (message.Contains("loop"))
                {
                    stringReturnMessage = videoDisplay.Loop(message);
                    videoDisplay2.Loop(message);
                    return stringReturnMessage;
                }

                else if (message.Contains("sync")) 
                {
                    stringReturnMessage = videoDisplay.SyncVideoTime();
                    return stringReturnMessage;
                }
                else if (message.Contains("move"))
                {
                    TimeSpan newTime;

                    message = message.Substring(message.IndexOf("-") + 1);
                    newTime = TimeSpan.Parse(message);
                    stringReturnMessage = videoDisplay.UpdateVideoTime(newTime);
                    videoDisplay2.UpdateVideoTime(newTime);
                    return stringReturnMessage;
                }

                else
                {
                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"RequestFailed\"}";
                    return stringReturnMessage;
                }
            }
            else
            {
                stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                return stringReturnMessage;
            }

        } 
    }
}