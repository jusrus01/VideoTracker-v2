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
using System.Timers;

namespace video_tracker_v2
{
    /// <summary>
    /// Interaction logic for VideosPage.xaml
    /// </summary>
    public partial class VideosPage : Page
    {
        private string path;

        private Window mainWindow;
        private bool isFullscreen = false;

        private Timer timer;

        private VideoPlayer player;

        public VideosPage()
        {
            InitializeComponent();
            mainWindow = Application.Current.MainWindow;
            videoView.Loaded += VideoView_Loaded;

            timer = new Timer(3000);
            timer.Elapsed += HideCursor;
        }

        public VideosPage(string path) : base()
        {
            this.path = path;
        }

        // test
        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            player = new VideoPlayer();

            videoView.MediaPlayer = player.mPlayer;

            player.mPlayer.TimeChanged += UpdateTimeSlider;
            player.PlayDemo();
        }

        private void UpdateTimeSlider(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            sliderTimeline.Dispatcher.Invoke(() =>
            {
                if (sliderTimeline.Maximum != Convert.ToDouble(player.currentMedia.Duration / 60))
                {
                    sliderTimeline.Maximum = Convert.ToDouble(player.currentMedia.Duration / 60);
                    // making sure volume slider is on some kind of value
                    sliderVolume.Value = 50; // default
                }

                sliderTimeline.Value = Convert.ToDouble(e.Time / 60);
            });
        }

        private void UpdateVideoVolume(object sender, RoutedEventArgs e)
        {
            // apply new audio value
            player.SetVolume(Convert.ToInt32((sender as Slider).Value));
        }

        private void HideCursor(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate 
            {
                Mouse.OverrideCursor = Cursors.None;
                HideUI();
            });
        }

        private void StopInactiveCounter(object sender, MouseEventArgs e)
        {
            timer.Stop();
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ShowSubtitlesContextMenu(object sender, MouseEventArgs e)
        {
            videoViewContextMenu.IsOpen = true;
        }

        private void MouseMoveE(object sender, MouseEventArgs e)
        {
            timer.Stop();
            Mouse.OverrideCursor = Cursors.Arrow;
            ShowUI(null, null);
            timer.Start();
        }

        private void ShowUI(object sender, MouseEventArgs e)
        {
            UIControls.Opacity = 100;
            sliderTimeline.Opacity = 100;
            volumeControls.Opacity = 100;
            rect.Opacity = 0.2;
        }

        private void HideUI()
        {
            UIControls.Opacity = 0;
            sliderTimeline.Opacity = 0;
            volumeControls.Opacity = 0;
            rect.Opacity = 0;
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

                isFullscreen = true;
            }
            else
            {
                mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                mainWindow.WindowState = WindowState.Normal;
                Grid.SetColumn(videoView, 2);
                Grid.SetColumnSpan(videoView, 1);
                Grid.SetRow(videoView, 0);
                Grid.SetRowSpan(videoView, 3);
                mainWindow.Focus();

                isFullscreen = false;
            }
        }

        private void PlayVideo(object sender, RoutedEventArgs e)
        {
            if(player.mPlayer.IsPlaying)
            {
                player.Pause();
            }
            else
            {
                player.Play();
            }
        }

        private void SetVideoToTime(object sender, RoutedEventArgs e)
        {
            // check if it's really a mouse click
            Slider s = sender as Slider;
            if (Convert.ToDouble(player.mPlayer.Time / 60) != s.Value)
            {
                player.SetTime(s.Value);
            }
        }
    }
}
