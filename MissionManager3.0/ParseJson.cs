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

                JsonIn = JsonIn.Replace('#', '\\');

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

            else if (JsonMessage[0] == "Play")
            {
                var JSONBody = JsonConvert.DeserializeObject<JSONPlayClass>(JsonMessage[1]);
                PropertyInfo[] JsonProperties = JSONBody.GetType().GetProperties();//get the properties of the class
                foreach (var prop in JsonProperties)
                {
                    string name = prop.Name as string; //get property name
                    string value = prop.GetValue(JSONBody, null) as string; //get property value

                    list.Add(name + ": " + value); //add both to list
                    return JsonReturn;
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
            List<string> fileNames = new List<string>();
            List<string> fileExtensions = new List<string>();





            for (int i = 0; i < directoryPaths.Length; i++)
            {

                string extension = System.IO.Path.GetExtension(directoryPaths[i]);
                string name = Path.GetFileName(directoryPaths[i]);
                string name2 = name.Substring(0, name.Length - extension.Length);
                name2 = name.Replace(@"\", @"/");
                directoryPaths[i] = directoryPaths[i].Replace(@"\", @"/");
                directoryNames.Add(name2);
            }

            for (int i = 0; i < filepaths.Length; i++)
            {
                string extension = System.IO.Path.GetExtension(filepaths[i]);
                string name = Path.GetFileName(filepaths[i]);
                string name2 = name.Substring(0, name.Length - extension.Length);
                filepaths[i] = filepaths[i].Replace(@"\", @"/");
                fileExtensions.Add(extension);
                fileNames.Add(name2);
            }
            string paths = "\"paths\":[";
            if (directoryPaths.Length > 1) //if/else blocks are because the last entry should not add a comma. this could be a dir or a file
            {
                if (filepaths.Length == 0)
                {
                    for (int i = 0; i < directoryPaths.Length - 1; i++) //directories builder
                    {
                        paths += "{\"fileName\":\"" + directoryNames[i] + "\",";

                        paths += "\"filePath\":\"" + directoryPaths[i] + "\"";

                        paths += "},";
                    }
                    int j = directoryPaths.Length - 1;
                    for (int i = 0; i < directoryPaths.Length; i++) //directories builder
                    {
                        paths += "{\"fileName\":\"" + directoryNames[i] + "\",";

                        paths += "\"filePath\":\"" + directoryPaths[i] + "\"";

                        paths += "}";
                    }
                }
                else
                {

                    for (int i = 0; i < directoryPaths.Length; i++) //directories builder
                    {
                        paths += "{\"fileName\":\"" + directoryNames[i] + "\",";

                        paths += "\"filePath\":\"" + directoryPaths[i] + "\"";

                        paths += "},";
                    }
                }
            }
            if (filepaths.Length > 0)
            {
                for (int i = 0; i < filepaths.Length - 1; i++)//files
                {
                    paths += "{\"fileName\":\"" + fileNames[i] + "\",";
                    paths += "\"fileExtension\":\"" + fileExtensions[i] + "\",";
                    paths += "\"filePath\":\"" + filepaths[i] + "\"";

                    paths += "},";
                }


                int j = filepaths.Length - 1;
                paths += "{\"fileName\":\"" + fileNames[j] + "\",";
                paths += "\"fileExtension\":\"" + fileExtensions[j] + "\",";
                paths += "\"filePath\":\"" + filepaths[j] + "\"";

                paths += "}";
            }

            //string JsonReturn = CreateJSON(files, directories);
            string JsonReturn = "{" + paths + "]}";
            return JsonReturn;
        }

    }
}

