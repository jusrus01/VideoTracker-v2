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
        public static string Path { get; set; }

        public static string[] LoadCategories()
        {
            if (File.Exists(Path))
            {
                string[] categories = File.ReadAllLines(Path);
                // check if files exists
                List<string> validCategories = new List<string>();
                foreach(string category in categories)
                {
                    if(File.Exists(category))
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
            File.AppendAllText(Path, categoryPath);
        }
    }
}
