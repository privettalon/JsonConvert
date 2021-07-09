using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace JsonConvert
{
    class JsonToString
    {
        public Dictionary<string, string> list { get; private set; }
        private JObject json;

        public JsonToString(string filePath)
        {
            list = new Dictionary<string, string>();
            string text;
            try
            {
                using (StreamReader streamreader = new StreamReader(filePath))
                {
                    text = streamreader.ReadToEnd();
                }

                DuplicatePropertyNameHandling duplicateFlag = (DuplicatePropertyNameHandling)2;
                JsonLoadSettings jsonLoadSettings = (duplicateFlag == DuplicatePropertyNameHandling.Replace)
                    ? null
                    : new JsonLoadSettings { DuplicatePropertyNameHandling = duplicateFlag };
                JObject.Parse(text, jsonLoadSettings);

                json = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(text);
                GetKeyValue(json, "");
            }
            catch (JsonReaderException)
            {
                Console.WriteLine("JSON isn't Valid");
            }
            catch(IOException)
            {
                Console.WriteLine("\nInput corect directory");
            }
        }

        private void GetKeyValue(JToken item, string pathString)
        {
            var currentJToken = item.First;
            var addingPath = currentJToken.Path.Length >= pathString.Length ? new string(currentJToken.Path.Skip(pathString.Length).ToArray()) + ":" : "";

            pathString += pathString.Length < 1 ? currentJToken.Path + ":" : addingPath;

            if (currentJToken.HasValues)
            {
                GetKeyValue(currentJToken, pathString);
            }
            else
            {
                pathString = pathString.Remove(pathString.Length - 1).Replace(":'", ":").Replace("']", "").Replace("\\'", "'").Replace("['", "");
                list.Add(pathString, currentJToken.ToString());
            }
            if (item.Next != null)
            {
                pathString = item.Parent == item.Root ? "" : new string(pathString.Substring(0, item.Parent.Path.Length + 1));
                GetKeyValue(item.Next, pathString);
            }
        }
    }
}
