using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_tracker_v2
{
    public class Video
    {
        public string Path { get; set; }

        public Video(string path)
        {
            this.Path = path;
        }
    }
}
