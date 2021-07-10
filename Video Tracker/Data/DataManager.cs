using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace video_tracker_v2
{
    /// <summary>
    /// This class is responsible
    /// for data saving and loading
    /// </summary>
    public static class DataManager
    {
        private static string mainPath { get; set; }  // this holds created categories
        private static string dataPath { get; set; }  // this holds specific video category data

        // DOENST WRITE CORRECTLY

        /// <summary>
        /// Opens file and reads paths to video folders
        /// </summary>
        /// <returns>Array of valid paths to video folders</returns>
        public static async Task<List<string>> LoadCategoriesAsync()
        {
            if (File.Exists(mainPath))
            {
                List<string> validCategories = new List<string>();

                using (StreamReader reader = new StreamReader(mainPath, Encoding.UTF8))
                {
                    string category;
                    
                    while((category = await reader.ReadLineAsync()) != null)
                    {
                        if (Directory.Exists(category))
                        {
                            validCategories.Add(category);
                        }
                    }
                }

                if(validCategories.Count == 0)
                {
                    return null;
                }

                return validCategories;
            }
            return null;
        }

        private static async Task<List<string>> ReadFileAsync(string path)
        {
            List<string> lines = new List<string>();

            string line;
            using (StreamReader reader = new StreamReader(path))
            {
                while((line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }
        
        /// <summary>
        /// Loads all videos from video directory
        /// </summary>
        /// <param name="categoryName">Name of videos folder</param>
        /// <returns>Loaded videos</returns>
        public static async Task<List<Video>> LoadVideosAsync(string categoryName)
        {
            // if file doesn't exists
            // create file
            // and return all videos with data
            List<Video> videos = new List<Video>();

            string dataFile = dataPath + '\\' + categoryName;
            List<string> allCategories = await LoadCategoriesAsync();
            string writtenPath = allCategories.Where(p => Path.GetFileName(p) == categoryName)
                .FirstOrDefault();

            if (File.Exists(dataFile))
            {
                List<string> lines = await ReadFileAsync(dataFile);

                string[] values;
                foreach (string line in lines)
                {
                    values = line.Split(';');

                    videos.Add(new Video(values[0], uint.Parse(values[1]),
                        uint.Parse(values[2]), bool.Parse(values[3])));
                }
                await WriteToFileAsync(lines, writtenPath);
            }
            else
            {
                //File.WriteAllText(dataFile, videoData.ToString());
                using (StreamWriter writer = new StreamWriter(dataFile))
                {
                    DirectoryInfo dicInfo = new DirectoryInfo(writtenPath);
                    foreach (FileInfo fi in dicInfo.GetFiles())
                    {
                        // checking if file has a valid extension
                        if (VideoPlayer.ValidExtensions.Contains(fi.Extension))
                        {
                            videos.Add(new Video(fi.FullName, 0, 0, false));

                            await writer.WriteLineAsync(string.Format("{0};{1};{2};{3}",
                                fi.FullName, "0", "0", "False"));
                        }
                    }
                }
            }
            return videos;
        }

        /// <summary>
        /// Open file and write new path entry
        /// </summary>
        /// <param name="categoryPath">Path to videos directory</param>
        public static void SaveCategory(string categoryPath)
        {
            File.AppendAllText(mainPath, categoryPath + '\n');
        }

        /// <summary>
        /// Checks if path entry already exists in data file
        /// </summary>
        /// <param name="entry">Path to videos directory</param>
        /// <returns>True if exists, false otherwise</returns>
        public static async Task<bool> EntryExistsAsync(string entry)
        {
            if (!File.Exists(mainPath))
                return false;

            var values = await ReadFileAsync(mainPath); 

            if (values.Contains(entry))
                return true;

            return false;
        }

        public static async Task<bool> RemoveCategoryFromFileAsync(string categoryPath)
        {
            List<string> allCategories = new List<string>();

            string category;
            // reconstructing list wihout specified category
            using (StreamReader reader = new StreamReader(mainPath))
            {
                category = await reader.ReadLineAsync();

                if (category != categoryPath)
                {
                    allCategories.Add(category);
                }
            }

            string _dataPath = $"{dataPath}//{Path.GetFileName(category)}";
            if(File.Exists(_dataPath))
            {
                File.Delete(_dataPath);
            }

            return await WriteToFileAsync(allCategories, mainPath);
        }

        private static async Task<bool> WriteToFileAsync(List<string> lines, string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    foreach (string line in lines)
                    {
                        await writer.WriteLineAsync(line);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
       
        /// <summary>
        /// Creates videos data folder, if it doesn't exist
        /// </summary>
        public static void Initialize(string infoFolderPath)
        {
            //DataPath = Path.GetDirectoryName(MainPath) + "\\videodata";
            mainPath = infoFolderPath;
            dataPath = "videodata";
            // check if it already exists
            if(!Directory.Exists(dataPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(dataPath);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }
        
        public static async void UpdateVideosDataAsync(string categoryName, List<Video> videos)
        {
            if (videos == null)
                return;

            string dataFile = dataPath + '\\' + categoryName;
            using (StreamWriter writer = new StreamWriter(dataFile))
            {
                foreach(Video v in videos)
                {
                    await writer.WriteLineAsync(v.ToString());
                }
            }
        }
    }
}
