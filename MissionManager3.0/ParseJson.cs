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
        private static VideoDisplay videoDisplay = null;

        public string InitialParsing(string JsonIn) //return JSON
        {
            if (JsonIn == "CONNECTION_ACTIVE")
            {
                string pies = JsonIn;
                return pies;
            }
            else
            {
                List<string> initialList = new List<string>();

             //  JsonIn = JsonIn.Replace('\\', '/');

                var JSONMessage = JsonConvert.DeserializeObject<JSONMessage>(JsonIn);
                PropertyInfo[] JsonProperties = JSONMessage.GetType().GetProperties();//get the properties of the class

                foreach (var prop in JsonProperties)
                {
                    string value = prop.GetValue(JSONMessage, null) as string; //get property value
                    initialList.Add(value);
                }

                string JsonReturn = ParseJsonMethod(initialList);
                return JsonReturn;
            }
        }



        public string ParseJsonMethod(List<string> JsonMessage) //will need to return JSON
        {
            string JsonReturn = "";
            List<string> list = new List<string>();
            if (JsonMessage[0] == "GetDirectories")
            {
                //will only receive file path to call a get directories method on to send back
                JsonReturn = GetDirectories(JsonMessage[1]);
                return JsonReturn;

            }

            else if (JsonMessage[0] == "LaunchVideo") 
            {
                JsonReturn = "{\"messageType\":\"LaunchVideo\",\"messageBody\":\"Launching Video " + JsonMessage[1] + "\"}";
            }

            else if (JsonMessage[0] == "VideoPlayer")
            {
                if (JsonMessage[1] == "Stop")
                {
                    JsonReturn = "{\"messageType\":\"AmendVideo\",\"messageBody\":\"Stopping\"}";
                }
                else if (JsonMessage[1] == "Pause")
                {
                    JsonReturn = "{\"messageType\":\"AmendVideo\",\"messageBody\":\"Pausing\"}";
                }
                else if (JsonMessage[1] == "Play")
                {
                    JsonReturn = "{\"messageType\":\"AmendVideo\",\"messageBody\":\"Playing\"}";
                }
            } 

            else if (JsonMessage[0] == "LaunchImage") //get path .jpg?
            {
                JsonReturn = "{\"messageType\":\"LaunchVideo\",\"messageBody\":\"Launching Image " + JsonMessage[1] + "\"}";
            }

            else if (JsonMessage[0] == "System")
            {

                if (JsonMessage[1] == "volumeup" || JsonMessage[1] == "volumedown")
                {
                    if (videoDisplay == null)
                    {
                        JsonReturn = "{\"messageType\":\"NoVideo\", \"messageBody\":\"\"}";
                    }
                    else if (JsonMessage[1] =="volumeup")
                    {
                        JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"Raising Volume\"}";  
                    }
                    else if (JsonMessage[1] == "volumedown")
                    {
                        JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"Lowering Volume\"}"; 
                    }
                }

                else if (JsonMessage[1] == "restartpc")
                {
                    JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"Restarting System\"}"; 
                }
                else if (JsonMessage[1] == "restartmissionmanager")
                {
                    JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"Restarting Mission Manager\"}"; 
                }
                else if (JsonMessage[1] == "shutdown")
                {
                    JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"Shutting Down System\"}";
                }


            }
            return JsonReturn;

        }

        string GetDirectories(string path)
        {
            var directoryPaths = Directory.GetDirectories(path);
            List<string> directoryNames = new List<string>();

            var filepaths = Directory.GetFiles(path);
            List<string> filePaths = filepaths.ToList();
            List<string> fileNames = new List<string>();
            List<string> fileExtensions = new List<string>();
            List<string> fileSizes = new List<string>();





            for (int i = 0; i < directoryPaths.Length; i++) //if no dirs that doesn't matter
            {

                string extension = System.IO.Path.GetExtension(directoryPaths[i]);
                string name = Path.GetFileName(directoryPaths[i]);
                string name2 = name.Substring(0, name.Length - extension.Length);
                name2 = name.Replace(@"\", @"/");
                directoryPaths[i] = directoryPaths[i].Replace(@"\", @"/");
                directoryNames.Add(name2);
            }
            if (filepaths.Length == 0) //if no files, need to create the list filePaths and put data in there to send back
            {
                fileExtensions.Add("none");
                fileNames.Add("none");
                string defaultPath = "this folder has no files";
                filePaths.Add(defaultPath);

            }
            else
            {
                for (int i = 0; i < filepaths.Length; i++)
                {
                    string extension = System.IO.Path.GetExtension(filePaths[i]);
                    string name = Path.GetFileName(filePaths[i]);
                    string name2 = name.Substring(0, name.Length - extension.Length);
                    filePaths[i] = filePaths[i].Replace(@"\", @"/");
                    fileExtensions.Add(extension);
                    fileNames.Add(name2);
                    fileSizes.Add(filePaths[i].Length.ToString());
                }
            }
            string paths = "\"paths\":[";

            for (int i = 0; i < directoryPaths.Length; i++) //directories builder
            {
                paths += "{\"fileName\":\"" + directoryNames[i] + "\",";

                paths += "\"filePath\":\"" + directoryPaths[i] + "\"";

                paths += "},";
            }


                for (int i = 0; i < filepaths.Length - 1; i++)//files
                {
                    paths += "{\"fileName\":\"" + fileNames[i] + "\",";
                    paths += "\"fileExtension\":\"" + fileExtensions[i] + "\",";
                    paths += "\"filePath\":\"" + filePaths[i] + "\"";
                    paths += "\"fileSizeInBytes\":\"" + fileSizes[i] + "\"";

                    paths += "},";
                }


                int j = filePaths.Count - 1; //needs to look at the list in case the array was empty (if a folder has no files, the list is created and added to)
                paths += "{\"fileName\":\"" + fileNames[j] + "\",";
                paths += "\"fileExtension\":\"" + fileExtensions[j] + "\",";
                paths += "\"filePath\":\"" + filePaths[j] + "\",";
                paths += "\"fileSizeInBytes\":\"" + fileSizes[j] + "\"";

                paths += "}";
            


            //string JsonReturn = CreateJSON(files, directories);
            string JsonReturn = "{" + paths + "]}";
            return JsonReturn;
        }

    }
}

