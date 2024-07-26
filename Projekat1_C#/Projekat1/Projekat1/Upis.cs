using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat1
{
    public class Upis
    {

        public static string GetFilePath(string fileName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            //Console.WriteLine("Putanja radnog direktorijuma "+currentDirectory);
            string parentDirectory = Directory.GetParent(currentDirectory).Parent.Parent.FullName; // Povratak na glavni direktorijum projekta 
            string path = Path.Combine(parentDirectory, fileName);

            //Console.WriteLine($"Putanja: \"{path}\".");
            return path;
        }
    }
}
