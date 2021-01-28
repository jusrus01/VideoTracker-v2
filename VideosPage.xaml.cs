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
using System.Windows.Media;
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
        private string categoryName;
        private string path;

        private Window mainWindow;
        private bool isFullscreen = false;

        private Timer timer;

        private VideoPlayer player;
        private List<Video> videos;
        private Button curPressedButton;
        private ProgressBar curProgressBar;

        public VideosPage()
        {

        }

        public VideosPage(string path)
        {
            InitializeComponent();

            this.path = path;
            this.categoryName = System.IO.Path.GetFileName(path);

            videos = DataManager.LoadVideos(this.categoryName);

            mainWindow = Application.Current.MainWindow;
            Application.Current.Exit += SaveVideoData;

            videoView.Loaded += VideoView_Loaded;

            timer = new Timer(3000);
            timer.Elapsed += HideCursor;
        }

        private void CreateVideoListings()
        {
            int count = 0;
            foreach(Video v in videos)
            {
                CreateButton(v, count);
                count++;
            }
        }

        private void CreateButton(Video v, int id)
        {
            TextBox textBox = new TextBox();
            textBox.Text = System.IO.Path.GetFileName(v.Path);
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.HorizontalContentAlignment = HorizontalAlignment.Left;
            textBox.FontSize = 12;
            textBox.Focusable = false;
            textBox.Background = Brushes.Transparent;
            textBox.BorderThickness = new Thickness(0);
            textBox.Cursor = Cursors.Arrow;
            textBox.Foreground = new SolidColorBrush(Color.FromRgb(249, 250, 249));

            Button btn = new Button();
            btn.Content = textBox;
            btn.Click += LoadNewVideo;
            btn.DataContext = id.ToString();
            btn.HorizontalAlignment = HorizontalAlignment.Stretch;
            btn.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            
            btn.Height = 50;
            btn.Background = new SolidColorBrush(Color.FromRgb(114, 117, 121));
            btn.BorderThickness = new Thickness(0);
            videoPanel.Children.Add(btn);

            ProgressBar bar = new ProgressBar();
            bar.Height = 20;
            bar.Maximum = v.Duration;
            bar.Value = v.CurrentTime;
            bar.BorderThickness = new Thickness(0);
            
            bar.DataContext = id.ToString();
            bar.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            bar.HorizontalAlignment = HorizontalAlignment.Stretch;
            bar.Margin = new Thickness(0, 0, 0, 5);
            bar.Background = new SolidColorBrush(Color.FromRgb(249, 250, 249));
            bar.Foreground = new SolidColorBrush(Color.FromRgb(105, 142, 122));

            videoPanel.Children.Add(bar);


            
            //Button btn = new Button();
            //btn.Click += Button_Click;
            //btn.DataContext = path;
            //btn.Width = 254;
            //btn.VerticalContentAlignment = VerticalAlignment.Center;
            //btn.HorizontalAlignment = HorizontalAlignment.Center;
            ////btn.FontSize = 26;
            ////btn.BorderThickness = new Thickness(0);
            //btn.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 32, 42));
            ////btn.Content = System.IO.Path.GetFileName(path);
            //btn.Content = textBox;
            //panelCategories.Children.Add(btn);
        }

        private void SaveVideoData(object sender, ExitEventArgs e)
        {
            if (player.currentVideo != null)
            {
                DataManager.UpdateVideoData(categoryName, player.currentVideo);
            }
        }

        private void LoadNewVideo(object sender, RoutedEventArgs e)
        {
            //player.Play(path,
            //    (sender as Button).Content.ToString());
            if(curPressedButton != null)
                curPressedButton.Background = new SolidColorBrush(Color.FromRgb(114, 117, 121));

            SaveVideoData(null, null);
            player.Play(videos.ElementAt(int.Parse((sender as Button).DataContext.ToString())));
            curPressedButton = (sender as Button);
            curPressedButton.Background = new SolidColorBrush(Color.FromRgb(164, 167, 171));

            curProgressBar = (ProgressBar)VisualTreeHelper.GetChild(videoPanel, int.Parse(curPressedButton.DataContext.ToString())+1);
        }

        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            player = new VideoPlayer();

            videoView.MediaPlayer = player.mPlayer;

            player.mPlayer.TimeChanged += UpdateTimeSlider;

            // making sure user can't
            // start playing video until video view is loaded
            CreateVideoListings();
        }

        private void GoToHomePage(object sender, RoutedEventArgs e)
        {
            // save current video data, if it was loaded
            if (player.mPlayer != null)
                DataManager.UpdateVideoData(categoryName, player.GetVideo());

            this.Content = null;
            player.Dispose();
            this.NavigationService.GoBack();
        }

        private void UpdateTimeSlider(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            sliderTimeline.Dispatcher.Invoke(() =>
            {
                if (sliderTimeline.Maximum != Convert.ToDouble(player.currentMedia.Duration / 60))
                {
                    sliderTimeline.Maximum = Convert.ToDouble(player.currentMedia.Duration / 60);
                    player.currentVideo.Duration = Convert.ToUInt32(player.currentMedia.Duration / 60);
                    // making sure volume slider is on some kind of value
                    sliderVolume.Value = 50; // default
                }

                sliderTimeline.Value = Convert.ToDouble(e.Time / 60);
                curProgressBar.Value = Convert.ToDouble(e.Time / 60);

                // updating video file too
                player.currentVideo.CurrentTime = Convert.ToUInt32(e.Time / 60);
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
