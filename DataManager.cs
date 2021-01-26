using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace video_tracker_v2
{
    // clear bad input (?)
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

        // TO DO: add files check
        public static List<Video> LoadVideos(string categoryName)
        {
            // if file doesn't exists
            // create file
            // and return all videos with data
            List<Video> videos = new List<Video>();

            if(File.Exists(DataPath + '\\' + categoryName))
            {
                string path = DataPath + '\\' + categoryName;
                string[] lines = File.ReadAllLines(path);

                foreach(string line in lines)
                {
                    string[] values = line.Split(';');
                    videos.Add(new Video(values[0]));
                }
            }
            else
            {
                StringBuilder videoData = new StringBuilder();

                string path = string.Empty;
                string[] categories = LoadCategories();
                foreach (string category in categories)
                {
                    if (Path.GetFileName(category) == categoryName)
                    {
                        path = category;
                        break;
                    }
                }

                DirectoryInfo dicInfo = new DirectoryInfo(path);
                foreach(FileInfo fi in dicInfo.GetFiles())
                {
                    videoData.AppendLine(string.Format("{0};{1};{2};{3};{4}",
                        fi.FullName, "0", "0", "False", "False"));
                    videos.Add(new Video(fi.FullName));
                }
                File.WriteAllText(DataPath + '\\' + categoryName, videoData.ToString());
            }
            return videos;
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
