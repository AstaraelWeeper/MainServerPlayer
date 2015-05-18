﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketTutorial.FormsServer.JSON;
using System.Reflection;
using System.IO;

namespace SocketTutorial.FormsServer
{
    class ParseJson
    {
       
        public List<string> InitialParsing(string Json)
        { 
            List<string> initialList = new List<string>();

            StreamReader JsonStream = new StreamReader(Json);
            string JsonExtract = JsonStream.ReadToEnd(); //extract JSON
            
            var JSONMessage = JsonConvert.DeserializeObject<JSONMessage>(JsonExtract);
            PropertyInfo[] JsonProperties = JSONMessage.GetType().GetProperties();//get the properties of the class

            foreach (var prop in JsonProperties)
            {
                string value = prop.GetValue(JSONMessage, null) as string; //get property value
                initialList.Add(value);
            }
            List<string> returnedList = ParseJsonMethod(initialList);
            return returnedList;
        }



        public List<string> ParseJsonMethod(List<string> JsonMessage)
        {
            List<string> list = new List<string>();
            if (JsonMessage[0] == "GetDirectories")
            {
                //will only receive file path to call a get directories method on to send back
                list.Add(JsonMessage[1]);
                
            }

            else if (JsonMessage[0] == "Play")
            {
                var JSONBody = JsonConvert.DeserializeObject<JSONPlayClass>(JsonMessage[1]);
                PropertyInfo[] JsonProperties = JSONBody.GetType().GetProperties();//get the properties of the class
                foreach (var prop in JsonProperties)
                {
                    string name = prop.Name as string; //get property name
                    string value = prop.GetValue(JSONBody, null) as string; //get property value

                    list.Add(name + ": " + value); //add both to list
                }
                
            }

            else if (JsonMessage[0] == "System")
            {
                var JSONBody = JsonConvert.DeserializeObject<JSONSystemClass>(JsonMessage[1]);
                PropertyInfo[] JsonProperties = JSONBody.GetType().GetProperties();//get the properties of the class
                foreach (var prop in JsonProperties)
                {
                    string name = prop.Name as string; //get property name
                    string value = prop.GetValue(JSONBody, null) as string; //get property value

                    list.Add(name + ": " + value); //add both to list
                }
                
            }
            return list;
        }

    }
}

