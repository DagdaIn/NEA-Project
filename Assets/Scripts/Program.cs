using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NEA_Saving_Test
{
    internal class Program
    {
        /// <summary>
        /// Testing the implementation of the subroutines
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            while(true)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                Console.WriteLine("Enter a name:");
                string name = Console.ReadLine();
                Console.WriteLine("Enter data:");
                string toSave = Console.ReadLine();
                Save(toSave, name, "test.txt");
                Read("test.txt", out data);

                WriteData(data);

                Console.WriteLine("Would you like to delete data? ");
                if (Console.ReadLine() == "y")
                {
                    Console.WriteLine("Enter the name: ");
                    name = Console.ReadLine();
                    DeleteData("test.txt", name);
                }
            }
        }

        /// <summary>
        /// Saves a name with data to a file
        /// </summary>
        /// <param name="toSave">The data to be saved</param>
        /// <param name="name">The name used to reference the data</param>
        /// <param name="filename">The name of the file, or the filepath</param>
        static void Save(string toSave, string name, string filename)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            Read(filename, out data);

            if (data.ContainsKey(name))
            {
                Console.WriteLine("Name already used!");
                return;
            }

            using (StreamWriter sw = new StreamWriter(filename, true))
            {
                sw.WriteLine($"{name} {toSave}");
            }
        }

        /// <summary>
        /// Reads the data from a file
        /// </summary>
        /// <param name="filename">The name of the file, or the filepath</param>
        /// <param name="data">The name and data as a dictionary</param>
        static void Read(string filename, out Dictionary<string, string> data)
        {
            data = new Dictionary<string, string>();
            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    data.Add(s.Split(' ')[0], ParseData(s.Split(' ')));
                }
            }
        }

        /// <summary>
        /// Logs the contents of <paramref name="data"/> to the console
        /// </summary>
        /// <param name="data">The dictionary containing all names and data</param>
        static void WriteData(Dictionary<string, string> data)
        {
            foreach (KeyValuePair<string, string> dataPiece in data)
            {
                Console.WriteLine($"{dataPiece.Key} {dataPiece.Value}");
            }
            Console.ReadKey();
            Console.Clear();
        }

        /// <summary>
        /// Returns a string of all but the 0th element of <paramref name="data"/>
        /// </summary>
        /// <param name="data">A string array of the data</param>
        /// <returns>A single string of the 1st to the last element of the data</returns>
        static string ParseData(string[] data)
        {
            string result = "";
            for (int i = 1; i < data.Length; i++)
            {
                result = $"{result} {data[i]}";
            }

            return result.Trim();
        }

        /// <summary>
        /// Deletes a line of data
        /// </summary>
        /// <param name="filename">The name of the file, or the filepath</param>
        /// <param name="name">The name associated with the data</param>
        static void DeleteData(string filename, string name)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            Read(filename, out data);

            if (!data.ContainsKey(name))
            {
                Console.WriteLine();
            }

            string toWrite = "";
            using (StreamReader sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line != null && line.Split(' ')[0] != name)
                    {
                        toWrite = $"{toWrite}{line}\n";
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(filename, false))
            {
                sw.Write(toWrite);
            }
        }
    }
}
