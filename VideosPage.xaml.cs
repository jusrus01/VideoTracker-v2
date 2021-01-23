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
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private Window mainWindow;
        private bool isFullscreen = false;

        public VideosPage()
        {
            InitializeComponent();
            mainWindow = Application.Current.MainWindow;
            videoView.Loaded += VideoView_Loaded;
        }
        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            _mediaPlayer.EnableMouseInput = false;
            videoView.MediaPlayer = _mediaPlayer;
            _mediaPlayer.Play(new Media(_libVLC, new Uri("C:\\Users\\achue\\Downloads\\ninenine\\test.mkv")));

        }


        private void PreviewLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) // if double click
            {
                EnterFullscreenMode();
            }
        }

        private void EnterFullscreenMode()
        {
            if (!isFullscreen)
            {
                mainWindow.WindowStyle = WindowStyle.None;
                mainWindow.WindowState = WindowState.Maximized;
                Grid.SetColumn(videoView, 0);
                Grid.SetColumnSpan(videoView, 10);
                Grid.SetRow(videoView, 0);
                Grid.SetRowSpan(videoView, 10);
                mainWindow.Focus();
                Mouse.OverrideCursor = Cursors.None;

                isFullscreen = true;
            }
            else
            {
                mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                mainWindow.WindowState = WindowState.Normal;
                Grid.SetColumn(videoView, 2);
                Grid.SetColumnSpan(videoView, 1);
                Grid.SetRow(videoView, 0);
                Grid.SetRowSpan(videoView, 2);
                mainWindow.Focus();
                Mouse.OverrideCursor = Cursors.Arrow;

                isFullscreen = false;
            }
        }
    }
}
