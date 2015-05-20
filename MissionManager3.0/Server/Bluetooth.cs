using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace SocketTutorial.FormsServer.Server
{
    class Bluetooth
    {
        void BluetoothConnect()
        {
            string androidTabletMAC = "68:05:71:AA:2B:C7"; // The MAC address of my phone, lets assume we know it

            BluetoothAddress addr = BluetoothAddress.Parse(androidTabletMAC);
            //var btEndpoint = new BluetoothEndPoint(addr, 1600);
            var btClient = new BluetoothClient();
            //btClient.Connect(btEndpoint);

            Stream peerStream = btClient.GetStream();

            StreamWriter sw = new StreamWriter(peerStream);
            sw.WriteLine("Hello World");
            sw.Flush();
            sw.Close();

            btClient.Close();
            btClient.Dispose();
           // btEndpoint = null;
        }
    }
}
