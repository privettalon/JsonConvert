
using System;
using System.IO;
using System.Reflection;


namespace JsonConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            //var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\")) +@"JsonExample\test.json";
            //var path = System.IO.Directory.GetCurrentDirectory() + @"\test2";

            Console.WriteLine("Please enter the full directory path");
            var path = Console.ReadLine();
            Console.WriteLine("Please enter a file name with an extension");
            var fileName = Console.ReadLine();


            var json = new JsonToString(@path + @"\" + fileName);
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
