using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;

namespace video_tracker_v2
{
    /// <summary>
    /// Interaction logic for VideosPage.xaml
    /// </summary>
    public partial class VideosPage : Page
    {
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;
        public VideosPage()
        {
            InitializeComponent();
            videoView.Loaded += VideoView_Loaded;
        }
        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            ////Core.Initialize();
            ////LibVLC vlc = new LibVLC();
            ////Media media = new Media(vlc, "C:\\Users\\achue\\Downloads\\ninenine\\test.mkv");
            ////MediaPlayer player = new MediaPlayer(vlc);
            ////view.MediaPlayer = player;
            ////player.Play();
            ///
            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            _mediaPlayer.EnableMouseInput = false;
            videoView.MediaPlayer = _mediaPlayer;
            _mediaPlayer.Play(new Media(_libVLC, new Uri("C:\\Users\\achue\\Downloads\\ninenine\\test.mkv")));

        }

        private void EnterFullscreenMode(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
