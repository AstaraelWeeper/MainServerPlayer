﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SocketTutorial.FormsServer
{
    public partial class MainView : Form
    {
        public MainView(string Json)
        {
            //test 
            
            //at the moment will display the secondary json
            InitializeComponent();
            ParseJson parseJson = new ParseJson();
            parseJson.InitialParsing(Json);
            
        }

    }
}
