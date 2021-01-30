using System;
using LibVLCSharp.Shared;
using LibVLCSharp.Shared.Structures;

namespace video_tracker_v2
{
    /// <summary>
    /// Handles video playing
    /// </summary>
    class VideoPlayer
    {
        // not sure if some of them are video files
        public readonly static string ValidExtensions = ".asx " +
            ".gxf" +
            ".m2v" +
            ".m3u" +
            ".m4v" +
            ".mpeg1" +
            ".mpeg2" +
            ".mts" +
            ".mxf" +
            ".ogm" +
            ".pls" +
            ".bup" +
            ".a52" +
            ".acc" +
            ".b4s" +
            ".divx" +
            ".dv" +
            ".flv" +
            ".m1v" +
            ".m2ts" +
            ".mkv" +
            ".mov" +
            ".mpeg4" +
            ".oma" +
            ".spx" +
            ".ts" +
            ".vlc" +
            ".vob" +
            ".xspf" +
            ".dat" +
            ".ifo" +
            ".part" +
            ".3g2" +
            ".avi" +
            ".mpeg" +
            ".mpg" +
            ".flac" +
            ".m4a" +
            ".mp1" +
            ".ogg" +
            ".wav" +
            ".xm" +
            ".3gp" +
            ".wmv" +
            ".wma" +
            ".mp4";

        public MediaPlayer mPlayer { get; set; }
        public Video currentVideo { get; set; }
        public Media currentMedia { get; set; }

        private LibVLC _libVLC;

        public VideoPlayer()
        {
            Core.Initialize();

            _libVLC = new LibVLC();
            mPlayer = new MediaPlayer(_libVLC);

            mPlayer.EnableMouseInput = false;
        }

        public void Pause()
        {
            mPlayer.Pause();
        }

        public void Play(Video video)
        {
            currentVideo = video;
            currentMedia = new Media(_libVLC, new Uri(video.Path));
            mPlayer.Media = currentMedia;
            Play();
        }

        public void Play()
        {
            mPlayer.Play();
        }

        public void SetTime(double time)
        {
            if (currentMedia != null)
            {
                long t = ((long)time) * 60;
                mPlayer.Time = t;
                currentVideo.CurrentTime = Convert.ToUInt32(time);
            }
        }

        public void SetVolume(int vol)
        {
            mPlayer.Volume = vol;
        }

        public void Dispose()
        {
            mPlayer.Dispose();
            _libVLC.Dispose();
        }

        public Video GetVideo()
        {
            return currentVideo;
        }

        public void MoveBy(int msTime)
        {
            if(msTime < 0 && mPlayer.Time - msTime < 0)
            {
                mPlayer.Time = 0;
                return;
            }

            mPlayer.Time += msTime;
        }
    }
}