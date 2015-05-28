using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InTheHand;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using System.IO;

namespace SocketTutorial.FormsServer
{
    class BluetoothServer
    {
        string JsonReturn;
        public delegate void ScreenWriterDelegate(string input);
        private ScreenWriterDelegate _screenWriterCallBT;

        public BluetoothServer (ScreenWriterDelegate screenWriterCallBT)
        {
            _screenWriterCallBT = screenWriterCallBT;
        }

        private ScreenWriterDelegate _screenWriterCall;
        Guid mUUID = new Guid("ef0e079e-de01-4080-bfcc-c8a24ecf69de");

        public void ServerConnectThread()
        {
            //serverStarted = true;
            _screenWriterCallBT("Server started, waiting for clients..");
            BluetoothListener blueListener = new BluetoothListener(mUUID);
            blueListener.Start();
            BluetoothClient conn = blueListener.AcceptBluetoothClient();
            _screenWriterCallBT("Client has connected");

            Stream mStream = conn.GetStream();
            while (true)
            {
                try
                {
                    byte[] recieved = new byte[1024];
                    mStream.Read(recieved, 0, recieved.Length);
                    string content = Encoding.ASCII.GetString(recieved);
                    _screenWriterCallBT("Recieved: " + content);

                    ParseJson parseJson = new ParseJson();
                    JsonReturn = parseJson.InitialParsing(content); //parse message

                    byte[] sent = Encoding.ASCII.GetBytes(JsonReturn);
                    mStream.Write(sent, 0, sent.Length);
                }
                catch (IOException exception)
                {
                    _screenWriterCallBT("Client has disconnected. Exception:" + exception + "\n");
                }
            }
        }

    }
}
