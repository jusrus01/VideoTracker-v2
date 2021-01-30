namespace video_tracker_v2
{
    /// <summary>
    /// Holds data about video file
    /// </summary>
    public class Video
    {
        public delegate void VideoCompletedDelegate();
        public VideoCompletedDelegate Completed;

        public string Path { get; set; }

        public uint CurrentTime
        {
            get { return currentTime; }

            set
            {
                this.currentTime = value;

                if (!Complete)
                {
                    // check if valid value was assigned
                    if (videoEndedAt == 0)
                        videoEndedAt = Duration - (Duration / 10);
                    
                    if (this.currentTime > videoEndedAt && Duration > 0)
                    {
                        Complete = true;
                        Completed();
                    }
                }
            }
        }

        public uint Duration { get; set; }
        public bool Complete { get; set; }

        private uint currentTime;
        private uint videoEndedAt;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to video file</param>
        /// <param name="curTime">Time in seconds</param>
        /// <param name="duration">Duration in seconds</param>
        /// <param name="complete">Video state</param>
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
