
using System;
using System.IO;
using System.Reflection;


namespace JsonConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")) +@"JsonExample\test.json";
            //var a = new CustomConvert(path);

            //var res = a.ConvertToDictionary();
            


            var json = new JsonToString(path);
            


            if (json.list != null)
            {
                foreach (var item in json.list)
                {
                    Console.WriteLine($"{item.Key} = \"{item.Value}\"");
                }
            }
            Console.ReadLine();
        }
    }
}
