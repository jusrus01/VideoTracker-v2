using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using LibVLCSharp.Shared;

namespace video_tracker_v2
{
    class VideoPlayer
    {
        public MediaPlayer mPlayer { get; set; }
        public Media currentMedia;

        private LibVLC _libVLC;

        public VideoPlayer()
        {
            Core.Initialize();

            _libVLC = new LibVLC();
            //currentMedia = new Media(_libVLC, new Uri("C:\\Users\\achue\\Downloads\\ninenine\\test.mkv"));
            mPlayer = new MediaPlayer(_libVLC);

            mPlayer.EnableMouseInput = false;
        }

        public void Pause()
        {
            mPlayer.Pause();
        }

        public void Play(string path, string videoName)
        {
            currentMedia = new Media(_libVLC, new Uri(path + '\\' + videoName));
            mPlayer.Media = currentMedia;

            Play();
        }

        public void Play()
        {
            mPlayer.Play();
        }

        public void SetTime(double time)
        {
            mPlayer.Time = (long)time * 60;
        }

        public void SetVolume(int vol)
        {
            mPlayer.Volume = vol;
        }

        public void Dispose()
        {
            _libVLC.Dispose();
            mPlayer.Dispose();
        }
    }
}
