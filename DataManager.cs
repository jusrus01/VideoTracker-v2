﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace video_tracker_v2
{
    /// <summary>
    /// This class is responsible
    /// for data saving and loading
    /// </summary>
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
        
        public static List<Video> LoadVideos(string categoryName)
        {
            // if file doesn't exists
            // create file
            // and return all videos with data
            List<Video> videos = new List<Video>();
            string dataFile = DataPath + '\\' + categoryName;

            if (File.Exists(dataFile))
            {
                string[] lines = File.ReadAllLines(dataFile);

                foreach (string line in lines)
                {
                    string[] values = line.Split(';');
                    videos.Add(new Video(values[0], uint.Parse(values[1]),
                        uint.Parse(values[2]), bool.Parse(values[3])));
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
                    // checking if file has a valid extension
                    if(VideoPlayer.ValidExtensions.Contains(fi.Extension))
                    {
                        videoData.AppendLine(string.Format("{0};{1};{2};{3}",
                            fi.FullName, "0", "0", "False"));
                        videos.Add(new Video(fi.FullName, 0, 0, false));
                    }
                }
                File.WriteAllText(dataFile, videoData.ToString());
            }
            return videos;
        }

        public static void SaveCategory(string categoryPath)
        {
            File.AppendAllText(MainPath, categoryPath + '\n');
        }

        public static bool EntryExists(string entry)
        {
            if (!File.Exists(MainPath))
                return false;

            string[] values = File.ReadAllLines(MainPath);

            if (values.Contains(entry))
                return true;
            return false;
        }

        public static void RemoveCategoryFromFile(string path)
        {
            // also remove file from data folder if exists
            string[] lines = File.ReadAllLines(MainPath);
            int i;
            string[] values;
            for (i = 0; i < lines.Length; i++)
            {
                values = lines[i].Split(';');
                if (values[0].Equals(path))
                {
                    break;
                }
            }
            lines = lines.Where(w => w != lines[i]).ToArray();

            File.WriteAllLines(MainPath, lines);
            string dataFile = DataPath + '\\' + Path.GetFileName(path);

            if (File.Exists(dataFile))
            {
                File.Delete(dataFile);
            }
        }

        public static void CreateDataFolder()
        {
            //DataPath = Path.GetDirectoryName(MainPath) + "\\videodata";
            DataPath = "videodata";
            // check if it already exists
            if(!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }
        }

        public static void UpdateVideoData(string categoryName, Video video)
        {
            if (video == null)
                return;

            string dataFile = DataPath + '\\' + categoryName;
            string[] lines = File.ReadAllLines(dataFile);
            int i;
            string[] values;

            for (i = 0; i < lines.Length; i++)
            {
                values = lines[i].Split(';');
                if (values[0].Equals(video.Path))
                {
                    lines[i] = video.ToString();
                    break;
                }
            }
            File.WriteAllLines(dataFile, lines);
        }
    }
}
