using System;
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
       
        public void InitialParsing(string JsonIn) //return JSON
        { 
            List<string> initialList = new List<string>();

           // JsonIn = JsonIn.Replace(@"\", string.Empty);

            var JSONMessage = JsonConvert.DeserializeObject<JSONMessage>(JsonIn);
            PropertyInfo[] JsonProperties = JSONMessage.GetType().GetProperties();//get the properties of the class

            foreach (var prop in JsonProperties)
            {
                string value = prop.GetValue(JSONMessage, null) as string; //get property value
                initialList.Add(value);
            }

            ParseJsonMethod(initialList);
        }



        public List<string> ParseJsonMethod(List<string> JsonMessage) //will need to return JSON
        {
            List<string> list = new List<string>();
            if (JsonMessage[0] == "GetDirectories")
            {
                //will only receive file path to call a get directories method on to send back
                GetDirectories(JsonMessage[1]);
                
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

        void GetDirectories(string path)
        {
            var directoryPaths = Directory.GetDirectories(path);
            List<string> directoryNames = new List<string>();
            List<string> directoryExtensions = new List<string>();
            var filepaths = Directory.GetFiles(path);
            List<string> fileNames = new List<string>();
            List<string> fileExtensions = new List<string>();

            List<JSONDirClass> files = new List<JSONDirClass>();
            List<JSONDirClass> directories = new List<JSONDirClass>();
            


            foreach (var dir in directoryPaths)
            {
                string extension = System.IO.Path.GetExtension(dir);
                string name = dir.Substring(0, dir.Length - extension.Length);
                directoryExtensions.Add(extension);
                directoryNames.Add(name);
            }

            foreach (var filepath in filepaths)
            {
                string extension = System.IO.Path.GetExtension(filepath);
                string name = filepath.Substring(0, filepath.Length - extension.Length);
                fileExtensions.Add(extension);
                fileNames.Add(name);
            }

            for (int i = 0; i < directoryPaths.Length; i++) //directories
            {
                JSONDirClass JsonDirClass = new JSONDirClass();
                JsonDirClass.name = directoryNames[i];
                JsonDirClass.path = directoryPaths[i];
                JsonDirClass.ext = directoryExtensions[i];
                JsonDirClass.File.Add(JsonDirClass.name); 
                JsonDirClass.File.Add(JsonDirClass.ext);
                JsonDirClass.File.Add(JsonDirClass.path);
                directories.Add(JsonDirClass);
            }

            for (int i = 0; i < filepaths.Length; i++)//files
            {
                JSONDirClass JsonDirClass = new JSONDirClass();
                JsonDirClass.name = fileNames[i];
                JsonDirClass.path = filepaths[i];
                JsonDirClass.ext = fileExtensions[i];
                JsonDirClass.File.Add(JsonDirClass.name);
                JsonDirClass.File.Add(JsonDirClass.ext);
                JsonDirClass.File.Add(JsonDirClass.path);
                files.Add(JsonDirClass);
            }

            CreateJSON(files, directories);
        }

        void CreateJSON(List<JSONDirClass> files, List<JSONDirClass> directories)
        {
            string filesOutput = JsonConvert.SerializeObject(files);
            string dirsOutput = JsonConvert.SerializeObject(directories);

            //create outerJSON

            JSONMessage jsonoutput = new JSONMessage();
            jsonoutput.MessageType = "Directories";
            jsonoutput.MessageBody = filesOutput + dirsOutput;

            string jsonReturn = JsonConvert.SerializeObject(jsonoutput);
            
        }

    }
}

