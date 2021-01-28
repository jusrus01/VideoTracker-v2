using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace video_tracker_v2
{

    // TO DO: on load subscribe
    //        and desubscribe on switch
    public class Video
    {
        public delegate void VideoCompletedDelegate();
        public VideoCompletedDelegate Completed;

        private uint currentTime;

        public string Path { get; set; }
        public uint CurrentTime
        {
            get { return currentTime; }

            set
            {
                this.currentTime = value;

                if(!Complete)
                    if (this.currentTime > Duration - (Duration / 10) && Duration > 0)
                    {
                        Complete = true;
                        Completed();
                    }
            }
        }

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
