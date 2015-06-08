using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketTutorial.FormsServer.JSON;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;



namespace SocketTutorial.FormsServer
{
    class ParseJson
    {
        //PowerpointHandler powerpointHandler = new PowerpointHandler();
        List<string> History = new List<string>();
        
        HandleImageViewers handleImageViewers = new HandleImageViewers();
        private Server.VideoFormActionDelegate _videoFormActionDelegate;

        public ParseJson(Server.VideoFormActionDelegate videoFormActionDelegate)
        {
            _videoFormActionDelegate = videoFormActionDelegate;
        }
       

        public string InitialParsing(string JsonIn) //return JSON
        {
            JsonIn = JsonIn.TrimEnd('\0');
            if (JsonIn.Contains( "CONNECTION_ACTIVE_BLUETOOTH" )|| JsonIn.Contains( "CONNECTION_ACTIVE_WIFI"))
            {
                string pies = JsonIn;
                return pies;
            }
            else
            {
                List<string> initialList = new List<string>();
                JsonIn = JsonIn.TrimEnd('\0');
                if (string.IsNullOrEmpty(JsonIn))
                {
                    return "";
                }
                else
                {
                    
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

            else if (JsonMessage[0] == "GetHistory")
            {
                JsonReturn = "{\"messageType\":\"GetHistory\",\"messageBody\":\"" + History.ToString() + "\"}";
            }

            else if (JsonMessage[0] == "LaunchVideo")
            {
                History.Add(JsonMessage[1]);
                JsonReturn = _videoFormActionDelegate(Server.VideoAction.InitialisePlayers, JsonMessage[1]);
                return JsonReturn;
            }

            else if (JsonMessage[0] == "VideoPlayer")
            {
                JsonReturn = _videoFormActionDelegate(Server.VideoAction.VideoPlayerControls, JsonMessage[1]);
                return JsonReturn;
            }

            else if (JsonMessage[0] == "LaunchImage") //get path .jpg?
            {
                History.Add(JsonMessage[1]);

                JsonReturn = handleImageViewers.InitialiseViewers(JsonMessage[1]); //get duration here.
                return JsonReturn;
            }

            else if (JsonMessage[0] == "ImageViewer")
            {
                JsonReturn = handleImageViewers.ImageViewerControls(JsonMessage[1]);
                return JsonReturn;
            }

            else if (JsonMessage[0] == "OpenPowerpoint")
            {
                JsonReturn = "";//powerpointHandler.initialisePowerpoint(JsonMessage[1]);
                return JsonReturn;
            }

            else if (JsonMessage[0] == "System")
            {


                if (JsonMessage[1] == "volumeup")
                {
                    //implement
                    JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"System Volume Raised\"}";
                    return JsonReturn;
                }
                else if (JsonMessage[1] == "volumedown")
                {
                    //implement
                    JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"System Volume Lowerer\"}";
                    return JsonReturn;
                }



                else if (JsonMessage[1] == "restartpc")
                {
                    Process.Start("shutdown", "/r /t 0");
                    JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"Restarting System\"}";
                    return JsonReturn;
                }
                else if (JsonMessage[1] == "restartmissionmanager")
                {
                    System.Diagnostics.Process.Start(Application.ExecutablePath); // to start new instance of application
                    //  this.Close(); //to turn off current app. needs updating
                    JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"Application Restarted\"}";
                    return JsonReturn;
                }
                else if (JsonMessage[1] == "shutdown")
                {
                    Process.Start("shutdown", "s /t 0");
                    JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"Shutting Down System\"}";
                    return JsonReturn;
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
                fileSizes.Add("0");

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
                    FileInfo fileInfo = new FileInfo(filePaths[i]);
                    long size = fileInfo.Length;
                    fileSizes.Add(size.ToString());
                }
            }
            string paths = "\"paths\":[";

            for (int i = 0; i < directoryPaths.Length; i++) //directories builder
            {
                paths += "{\"fileName\":\"" + directoryNames[i] + "\",";

                paths += "\"filePath\":\"" + directoryPaths[i] + "\"";

                paths += "},";
            }

            if (filePaths.Count > 1)
            {

                for (int i = 0; i < filePaths.Count - 1; i++)//files
                {
                    paths += "{\"fileName\":\"" + fileNames[i] + "\",";
                    paths += "\"fileExtension\":\"" + fileExtensions[i] + "\",";
                    paths += "\"filePath\":\"" + filePaths[i] + "\",";
                    paths += "\"fileSizeInBytes\":\"" + fileSizes[i] + "\"";

                    paths += "},";
                }


                int j = filePaths.Count - 1; //needs to look at the list in case the array was empty (if a folder has no files, the list is created and added to)
                paths += "{\"fileName\":\"" + fileNames[j] + "\",";
                paths += "\"fileExtension\":\"" + fileExtensions[j] + "\",";
                paths += "\"filePath\":\"" + filePaths[j] + "\",";
                paths += "\"fileSizeInBytes\":\"" + fileSizes[j] + "\"";

                paths += "}";
            }
            else if (filePaths.Count == 1)
            {
                paths += "{\"fileName\":\"" + fileNames[0] + "\",";
                paths += "\"fileExtension\":\"" + fileExtensions[0] + "\",";
                paths += "\"filePath\":\"" + filePaths[0] + "\",";
                paths += "\"fileSizeInBytes\":\"" + fileSizes[0] + "\"";

                paths += "}";
            }



            //string JsonReturn = CreateJSON(files, directories);
            string JsonReturn = "{" + paths + "]}";
            return JsonReturn;
        }

    }
}

