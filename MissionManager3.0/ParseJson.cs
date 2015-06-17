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
        List<string> history = new List<string>();
        string storedHistoryFilePath = @"C:\Users\Public\MissionManagerHistory.txt";
        
        private Server.VideoFormActionDelegate _videoFormActionDelegate;
        private Server.ImageFormActionDelegate _imageFormActionDelegate;

        public ParseJson(Server.VideoFormActionDelegate videoFormActionDelegate, Server.ImageFormActionDelegate imageFormActionDelegate)
        {
            _videoFormActionDelegate = videoFormActionDelegate;
            _imageFormActionDelegate = imageFormActionDelegate;
            if (!File.Exists(storedHistoryFilePath))
            {
                File.Create(storedHistoryFilePath).Dispose();
            }
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
            string JsonReturn = "failed at ParseJson";
            List<string> list = new List<string>();
            if (JsonMessage[0] == "GetDirectories")
            {
                JsonReturn = GetDirectories(JsonMessage[1]);
                return JsonReturn;

            }

                else if(JsonMessage[0] == "GetDrives")
            {
                JsonReturn = GetDrives();              
            }

            else if (JsonMessage[0] == "GetHistory")
            {
                JsonReturn = GetHistory();    
            }

            else if (JsonMessage[0] == "LaunchVideo")
            {
                AdjustHistory(JsonMessage[1]);            
                JsonReturn = _videoFormActionDelegate(Server.VideoAction.InitialisePlayers, JsonMessage[1]);
                return JsonReturn;
            }

            else if (JsonMessage[0] == "VideoPlayer")
            {
                JsonReturn = _videoFormActionDelegate(Server.VideoAction.VideoPlayerControls, JsonMessage[1]);
                return JsonReturn;
            }

            else if (JsonMessage[0] == "LaunchImage")
            {
                AdjustHistory(JsonMessage[1]);
                JsonReturn = _imageFormActionDelegate(Server.ImageAction.InitialiseImages, JsonMessage[1]); //get duration here.
                return JsonReturn;
            }

            else if (JsonMessage[0] == "ImageViewer")
            {
                JsonReturn = _imageFormActionDelegate(Server.ImageAction.ImagePlayerControls, JsonMessage[1]);
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
                    JsonReturn = "{\"messageType\":\"System\",\"messageBody\":\"System Volume Lowered\"}";
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
                     //this.Close(); //to turn off current app. needs updating
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

        void AdjustHistory(string filePath)
        {
            history.Add(filePath);
            if (history.Count() <= 10)
            {
                System.IO.File.WriteAllLines(storedHistoryFilePath, history); //save over file if up to 10 paths
            }
            else //when history.Count = 11
            {
                for (int i = 0; i < history.Count(); i++) //i go up to 10
                {
                    history[i] = history[i + 1]; //move them all up one
                }
                history.RemoveAt(history.Count()); //remove the last one
                System.IO.File.WriteAllLines(storedHistoryFilePath, history); //save over file if up to 10 paths
            }
        }
        string GetHistory()
        {
            string historyString = "{\"historyPaths\":[";
            if(!File.Exists(storedHistoryFilePath))
            {
                history = System.IO.File.ReadAllLines(storedHistoryFilePath).ToList();
            }
            if (history.Count() > 1)
            {
                int existsCount = 0;

                for (int i = 0; i < history.Count() - 1; i++)
                {
                    if (File.Exists(history[i]))
                    {
                        existsCount++;
                        FileInfo fileInfo = new FileInfo(history[i]);
                        long size = fileInfo.Length;
                        historyString += "{\"fileName\":\"" + Path.GetFileNameWithoutExtension(history[i]) + "\",";
                        historyString += "\"fileExtension\":\"" + Path.GetExtension(history[i]) + "\",";
                        historyString += "\"filePath\":\"" + history[i] + "\",";
                        historyString += "\"fileSizeInBytes\":\"" + size.ToString() + "\"";
                        historyString += "},";
                    }
                }
                int j = history.Count - 1;
                if (File.Exists(history[j]))
                {
                    existsCount++;
                    FileInfo fileInfoJ = new FileInfo(history[j]);
                    long sizeJ = fileInfoJ.Length;
                    historyString += "{\"fileName\":\"" + history[j] + "\",";
                    historyString += "\"fileExtension\":\"" + history[j] + "\",";
                    historyString += "\"filePath\":\"" + history[j] + "\",";
                    historyString += "\"fileSizeInBytes\":\"" + sizeJ.ToString() + "\"";

                    historyString += "}]}";
                }
                if(existsCount == 0)
                {
                    historyString += "{\"fileName\":\" none \",";
                    historyString += "\"fileExtension\":\" none \",";
                    historyString += "\"filePath\":\" none \",";
                    historyString += "\"fileSizeInBytes\":\" 0 \"";
                    historyString += "}]}";
                }
            }
            else if (history.Count == 1)
            {
                if (File.Exists(history[0]))
                {
                    FileInfo fileInfo = new FileInfo(history[0]);
                    long size = fileInfo.Length;
                    historyString += "{\"fileName\":\"" + history[0] + "\",";
                    historyString += "\"fileExtension\":\"" + history[0] + "\",";
                    historyString += "\"filePath\":\"" + history[0] + "\",";
                    historyString += "\"fileSizeInBytes\":\"" + size.ToString() + "\"";

                    historyString += "}]}";
                }
                else
                {
                    historyString += "}]}";
                }
            }
            else
            {                
                    historyString += "]}";
            }

            return historyString;
        }
        string GetDrives()
        {
            var driveInfo = DriveInfo.GetDrives();
            List<string> driveInfoStr = new List<string>();
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory).Replace("\\", "/");
            string drivePaths = "{\"drives\":[";

            if (drivePaths.Count() > 0)
            {

                for (int i = 0; i < driveInfo.Count(); i++)
                {
                        driveInfoStr.Add(driveInfo[i].ToString().Replace("\\", "/"));
                        drivePaths += "{\"driveName\":\"" + driveInfoStr[i] + "\",";
                        drivePaths += "\"driveType\":\"" + driveInfo[i].DriveType.ToString() + "\",";
                        drivePaths += "\"drivePath\":\"" + driveInfoStr[i] + "\",";
                        if (driveInfo[i].IsReady)
                        {
                            drivePaths += "\"driveReady\":\"" + "yes" + "\"";
                        }
                        else
                        {
                            drivePaths += "\"driveReady\":\"" + "no" + "\"";
                        }
                        drivePaths += "},";

                    
                }

                 
                drivePaths += "{\"driveName\":\"Desktop\",";
                drivePaths += "\"driveType\":\"N/A\",";
                drivePaths += "\"drivePath\":\"" + desktop + "\"";
                drivePaths += "}]}";
            }

            else
            {
                drivePaths += "{\"driveName\":\" none \",";
                drivePaths += "\"driveType\":\" none \",";
                drivePaths += "\"drivePath\":\" none \"";
                drivePaths += "\"driveReady\":\"N/A\"";
                drivePaths += "}]}";
            }
            return drivePaths;
        }
        string GetDirectories(string path)
        {
            string JsonReturn;
            if (Directory.Exists(path))
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
                    string defaultPath = path;
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
                    if (directoryNames[i] == "$RECYCLE.BIN" || directoryNames[i] == "Documents and Settings")
                    {
                    }
                    else
                    {
                        paths += "{\"fileName\":\"" + directoryNames[i] + "\",";

                        paths += "\"filePath\":\"" + directoryPaths[i] + "\"";

                        paths += "},";
                    }
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
              JsonReturn = "{" + paths + "]}";
            }
            else //if dir doesn't exist
            {
                JsonReturn = "{\"directory not found\"}";//this ok?
            }
            return JsonReturn;
        }

    }
}

