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
        public uint CurrentTime { get; set; }
        public uint Duration { get; set; }
        public bool Complete { get; set; }

        public Video(string path, uint curTime, uint duration, bool complete)
        {
            Path = path;
            CurrentTime = curTime;
            Duration = duration;
            Complete = complete;
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3}",
                Path, CurrentTime, Duration, Complete);
        }
    }
}
