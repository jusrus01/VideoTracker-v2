namespace video_tracker_v2
{
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
