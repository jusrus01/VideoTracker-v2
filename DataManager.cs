using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace video_tracker_v2
{
    public static class DataManager
    {
        public static string MainPath { get; set; }
        public static string DataPath { get; set; }

        public static string[] LoadCategories()
        {
            if (File.Exists(MainPath))
            {
                string[] categories = File.ReadAllLines(MainPath);
                // check if files exists
                List<string> validCategories = new List<string>();
                foreach(string category in categories)
                {
                    if(Directory.Exists(category))
                    {
                        validCategories.Add(category);
                    }
                }
                if (validCategories.Count == 0)
                    return null;

                return validCategories.ToArray<string>();
            }
            return null;
        }

        public static void SaveCategory(string categoryPath)
        {
            File.AppendAllText(MainPath, categoryPath + '\n');
        }

        public static bool EntryExists(string entry)
        {
            string[] values = File.ReadAllLines(MainPath);
            if (values.Contains(entry))
                return true;
            return false;
        }

        public static void CreateDataFolder()
        {
            DataPath = Path.GetDirectoryName(MainPath) + "\\videodata";
            // check if it already exists
            if(!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }
        }
    }
}
