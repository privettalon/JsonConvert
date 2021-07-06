using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace JsonConvert
{
    class JsonToString
    {
        public Dictionary<string,string> list { get; private set; }
        private JObject json;
        public JsonToString(string filePath)
        {
            list = new Dictionary<string, string>();
            string text;
            
            using (StreamReader streamreader = new StreamReader(filePath))
            {
                text = streamreader.ReadToEnd();
            }
            json = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(text);
            GetKeyValue(json);
        }

        private void GetKeyValue(JToken item)
        {
            var temp = item.First;
            
            if(temp.HasValues)
            {
                GetKeyValue(temp);
           
            }
            else 
            {
                list.Add(temp.Path.Replace('.',':'), temp.ToString());
            }

            if (item.Next!=null)
            {
                GetKeyValue(item.Next);
            }
        }
    }
}
