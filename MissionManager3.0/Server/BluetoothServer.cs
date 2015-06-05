using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using System.IO;
using System.Threading;

namespace SocketTutorial.FormsServer
{
    class BluetoothServer
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        string JsonReturn;
        public delegate void ScreenWriterDelegate(string input);
        private ScreenWriterDelegate _screenWriterCallBT;

        public BluetoothServer (ScreenWriterDelegate screenWriterCallBT)
        {
            _screenWriterCallBT = screenWriterCallBT;
        }

        Guid mUUID = new Guid("ef0e079e-de01-4080-bfcc-c8a24ecf69de");

        public void ServerConnectThread()
        {
            //serverStarted = true;
            _screenWriterCallBT("Bluetooth Server started, waiting for clients..");
            BluetoothListener blueListener = new BluetoothListener(mUUID); 
            blueListener.Start();
            BluetoothClient conn = blueListener.AcceptBluetoothClient();
            _screenWriterCallBT("Bluetooth Client has connected");
            bool disconnectedClient = false;
            
            while (true)
            {
              //  allDone.Reset();
                try
                {
                    if (disconnectedClient)
                    {
                        _screenWriterCallBT("Bluetooth server waiting for client");
                        conn = blueListener.AcceptBluetoothClient();
                        disconnectedClient = false;
                    }

                    Stream mStream = conn.GetStream();
                    byte[] recieved = new byte[1024];
                    mStream.Read(recieved, 0, recieved.Length);
                    string content = Encoding.ASCII.GetString(recieved).TrimEnd('\0');
                    if (content.Length == 0)
                    {
                        disconnectedClient = true;
                    }

                    _screenWriterCallBT("Recieved: " + content + "via bluetooth");

                    ParseJson parseJson = new ParseJson();
                    JsonReturn = parseJson.InitialParsing(content); //parse message

                    byte[] sent = Encoding.ASCII.GetBytes(JsonReturn);
                    mStream.Write(sent, 0, sent.Length);
                    string messageSent = Encoding.ASCII.GetString(sent);
                    _screenWriterCallBT("sent via Bluetooth: " + messageSent);
                }
                catch (IOException exception)
                {
                    _screenWriterCallBT("Bluetooth Client has disconnected. Exception:" + exception + "\n");
                }
               // allDone.WaitOne();
            }
        }

    }
}
