﻿using System;
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
using System.IO;

namespace SocketTutorial.FormsServer
{
    public partial class Server : Form
    {
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

        int vid1X = 0;
        int vid2X = 0;
        int pic1X = 0;
        int pic2X = 0;
        string storedMessagesFilePath = @"C:\Users\Public\MissionManagerLog.txt";
        List<string> MessageLog = new List<string>();


        public Server()
        {
            InitializeComponent();
            if (!File.Exists(storedMessagesFilePath))
            {
                File.Create(storedMessagesFilePath).Dispose();
            }
            getScreenSize();
            SystemVolume sysvol = new SystemVolume();
            sysvol.SysVolSetup();
            screenWriterDelegate = new AsynchronousSocketListener.ScreenWriterDelegate(WriteToScreen);
            screenWriterDelegateBT = new BluetoothServer.ScreenWriterDelegate(WriteToScreenBT);
            videoFormActionDelegate = new VideoFormActionDelegate(PerformVideoAction);
            imageFormActionDelegate = new ImageFormActionDelegate(PerformImageAction);
            LoadWifiAndBTListeners();
        }


        private void getScreenSize()
        {
            Screen thisScreen = Screen.PrimaryScreen;
            resolutionHight = thisScreen.WorkingArea.Height;
            resolutionWidth = thisScreen.WorkingArea.Width;
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
                MessageLog.Add("\r\n" + message);
                System.IO.File.WriteAllLines(storedMessagesFilePath, MessageLog);
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
                MessageLog.Add("\r\n" + message);
                System.IO.File.WriteAllLines(storedMessagesFilePath, MessageLog);
            }
        }

        void LoadWifiAndBTListeners()
        {
            listener = new AsynchronousSocketListener(screenWriterDelegate, videoFormActionDelegate, imageFormActionDelegate);
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

        public void RotateVideosRight()
        {

            if (videoDisplay.currentFrontDirection < 3) //0 to 3 range 
            {
                videoDisplay.currentFrontDirection++;
            }
            else if (videoDisplay.currentFrontDirection == 3)
            {
                videoDisplay.currentFrontDirection = 0;
            }
            int leftDirection = videoDisplay.currentFrontDirection;
            vid1X = leftDirection * resolutionWidth;


            if (videoDisplay2.currentFrontDirection < 0) //0 to -3  range 
            {
                videoDisplay2.currentFrontDirection++;
            }
            else if (videoDisplay2.currentFrontDirection == 0)
            {
                videoDisplay2.currentFrontDirection = -3;
            }
            int rightDirection = videoDisplay2.currentFrontDirection;
            vid2X = rightDirection * resolutionWidth;
        }

        public void RotateImagesRight()
        {

            if (imageDisplay.currentFrontDirection < 3) //0 to 3 range 
            {
                imageDisplay.currentFrontDirection++;
            }
            else if (imageDisplay.currentFrontDirection == 3)
            {
                imageDisplay.currentFrontDirection = 0;
            }
            int leftDirection = imageDisplay.currentFrontDirection;
            pic1X = leftDirection * resolutionWidth;


            if (imageDisplay2.currentFrontDirection < 0) //0 to -3  range 
            {
                imageDisplay2.currentFrontDirection++;
            }
            else if (imageDisplay2.currentFrontDirection == 0)
            {
                imageDisplay2.currentFrontDirection = -3;
            }
            int rightDirection = imageDisplay2.currentFrontDirection;
            pic2X = rightDirection * resolutionWidth;
        }

        public string PerformVideoAction(VideoAction action, string message)
        {
            string JsonReturn = "failed at PVA";

            if (txtServer.InvokeRequired)
            {
                return Invoke(videoFormActionDelegate, action, message).ToString();
            }

            else
            {
                if (action == VideoAction.InitialisePlayers)
                {

                    if (videoDisplay != null)
                    {
                        videoDisplay.Dispose();
                        videoDisplay2.Dispose();
                        videoDisplay = null;
                        videoDisplay2 = null;
                    }
                    videoDisplay = new VideoDisplay(1, message); //expecting direction to reset to 0 if new one opened
                    videoDisplay2 = new VideoDisplay(2, message);
                    if (imageDisplay != null)
                    {
                        imageDisplay.Dispose();
                        imageDisplay2.Dispose();
                        imageDisplay = null;
                        imageDisplay2 = null;
                    }



                    videoDisplay.Width = screens * resolutionWidth;
                    videoDisplay2.Width = screens * resolutionWidth;
                    videoDisplay.Height = resolutionHight;
                    videoDisplay2.Height = resolutionHight;
                    videoDisplay.StartPosition = FormStartPosition.Manual;
                    videoDisplay.Location = new Point(0, 0);
                    videoDisplay2.StartPosition = FormStartPosition.Manual;
                    videoDisplay2.Location = new Point(0, 0);
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
                    if (imageDisplay != null)
                    {
                        imageDisplay.Dispose();
                        imageDisplay2.Dispose();
                        imageDisplay = null;
                        imageDisplay2 = null;
                    }

                    imageDisplay = new ImageDisplay(1, message); //expecting direction to reset to 0 if new one opened
                    imageDisplay2 = new ImageDisplay(2, message);
                    if (videoDisplay != null)
                    {
                        videoDisplay.Dispose();
                        videoDisplay2.Dispose();
                        videoDisplay = null;
                        videoDisplay2 = null;
                    }


                    imageDisplay.Width = screens * resolutionWidth;
                    imageDisplay2.Width = screens * resolutionWidth;
                    imageDisplay.Height = resolutionHight;
                    imageDisplay2.Height = resolutionHight;
                    imageDisplay.StartPosition = FormStartPosition.Manual;
                    imageDisplay.Location = new Point(0, 0);
                    imageDisplay2.StartPosition = FormStartPosition.Manual;
                    imageDisplay2.Location = new Point(0, 0);
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

            if (videoDisplay != null || imageDisplay != null)
            {

                if (message.Contains("stop"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = videoDisplay.Stop();
                        videoDisplay2.Stop();
                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";                        
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("pause"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = videoDisplay.Pause();
                        videoDisplay2.Pause();                        
                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("play"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = videoDisplay.Play();
                        videoDisplay2.Play();
                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }

                else if (message.Contains("volumeup"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = videoDisplay.IncreaseVolume();
                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("volumedown"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = videoDisplay.DecreaseVolume();
                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("loop"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = videoDisplay.Loop(message);
                        videoDisplay2.Loop(message);
                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }

                else if (message.Contains("sync"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = videoDisplay.SyncVideoTime();
                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("move"))
                {
                    if (videoDisplay != null)
                    {
                        TimeSpan newTime;

                        message = message.Substring(message.IndexOf("-") + 1);
                        newTime = TimeSpan.Parse(message);
                        stringReturnMessage = videoDisplay.UpdateVideoTime(newTime);
                        videoDisplay2.UpdateVideoTime(newTime);
                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }


                else if (message.Contains("rotate right"))
                {
                    if (videoDisplay != null)
                    {
                        RotateVideosRight();
                        stringReturnMessage = videoDisplay.getFacingDirectionJSON();
                        videoDisplay.StartPosition = FormStartPosition.Manual;
                        videoDisplay.Location = new Point(vid1X, 0);
                        videoDisplay2.StartPosition = FormStartPosition.Manual;
                        videoDisplay2.Location = new Point(vid2X, 0);
                        if (videoDisplay.WindowState == FormWindowState.Maximized)
                        {
                            videoDisplay2.Show();
                            videoDisplay.Show();
                            videoDisplay2.Show();
                        }
                    }
                    else if (imageDisplay != null)
                    {
                        stringReturnMessage = imageDisplay.getFacingDirectionJSON();
                        RotateImagesRight();
                        imageDisplay.StartPosition = FormStartPosition.Manual;
                        imageDisplay.Location = new Point(pic1X, 0);
                        imageDisplay2.StartPosition = FormStartPosition.Manual;
                        imageDisplay2.Location = new Point(pic2X, 0);
                        if (imageDisplay.WindowState == FormWindowState.Maximized)
                        {
                            imageDisplay2.Show();
                            imageDisplay.Show();
                            imageDisplay2.Show();
                        }
                    }

                    return stringReturnMessage;

                }
                else if (message.Contains("minimize"))
                {
                    if (videoDisplay != null)
                    {
                        videoDisplay.Hide();
                        videoDisplay2.Hide();
                    }
                    else if (imageDisplay != null)
                    {
                        imageDisplay.Hide();
                        imageDisplay2.Hide();
                    }
                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Minimized\"}";
                    return stringReturnMessage;
                }
                else if (message.Contains("maximize"))
                {
                    if (videoDisplay != null)
                    {
                        videoDisplay.Show();
                        videoDisplay2.Show();
                    }
                    else if (imageDisplay != null)
                    {
                        imageDisplay.Show();
                        imageDisplay2.Show();
                    }
                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Maximized\"}";
                    return stringReturnMessage;
                }

                else
                {
                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"RequestNotRecognised\"}";
                    return stringReturnMessage;
                }
            }
            else
            {
                stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Players\"}";
                return stringReturnMessage;
            }

        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            txtServer.Text = "";
        }

        private void Server_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}