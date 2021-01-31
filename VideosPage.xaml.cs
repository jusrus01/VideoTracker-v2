using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LibVLCSharp.Shared;
using System.Timers;
using LibVLCSharp.Shared.Structures;
using Microsoft.WindowsAPICodePack.Dialogs;

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

        private SolidColorBrush videoCompletedBrush;
        private SolidColorBrush videoBrush;
        private SolidColorBrush activeVideoBrush;
        private SolidColorBrush textBoxBrush;
        private SolidColorBrush textBoxCompletedBrush;
        private SolidColorBrush orangeBrush;
        private SolidColorBrush barBackBrush;

        private ImageBrush playImageBrush;
        private ImageBrush pauseImageBrush;

        private int fontSize = 12;

        private Thickness borderNone;
        private Thickness onlyBottomMargin;

        public VideosPage(string path)
        {
            InitializeComponent();

            this.path = path;
            categoryName = System.IO.Path.GetFileName(path);
            // init style values
            videoCompletedBrush = new SolidColorBrush(Color.FromRgb(235, 94, 40));
            textBoxBrush = new SolidColorBrush(Color.FromRgb(235, 232, 222));
            textBoxCompletedBrush = new SolidColorBrush(Color.FromRgb(255, 215, 162));
            videoBrush = new SolidColorBrush(Color.FromRgb(64, 61, 57));
            orangeBrush = new SolidColorBrush(Color.FromRgb(235, 94, 40));
            activeVideoBrush = new SolidColorBrush(Color.FromRgb(104, 101, 97));
            barBackBrush = new SolidColorBrush(Color.FromRgb(44, 41, 37));
            borderNone = new Thickness(0);
            onlyBottomMargin = new Thickness(0, 0, 0, 5);

            playImageBrush = new ImageBrush(new BitmapImage(new Uri(@"..\..\Resources\play-button.png", UriKind.Relative)));
            pauseImageBrush = new ImageBrush(new BitmapImage(new Uri(@"..\..\Resources\pause-button.png", UriKind.Relative)));

            videos = DataManager.LoadVideos(categoryName);

            mainWindow = Application.Current.MainWindow;
            Application.Current.Exit += SaveVideoData;
            mainWindow.Title = "Video Tracker - " + categoryName;

            videoView.Loaded += VideoView_Loaded;

            timer = new Timer(3000);
            timer.Elapsed += HideCursor;

            // subscribe to key up event
            mainWindow.KeyUp += HandleKeyUp;
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

        private void OnVideoComplete()
        {
            curPressedButton.Background = videoCompletedBrush;
            ((TextBox)curPressedButton.Content).Foreground = textBoxCompletedBrush;
        }

        private void CreateButton(Video v, int id)
        {
            TextBox textBox = new TextBox();

            textBox.Text = System.IO.Path.GetFileNameWithoutExtension(v.Path);
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.HorizontalContentAlignment = HorizontalAlignment.Left;
            textBox.FontSize = fontSize;
            textBox.Focusable = false;
            textBox.Background = Brushes.Transparent;
            textBox.BorderThickness = borderNone;
            textBox.Cursor = Cursors.Arrow;
            textBox.Foreground = textBoxBrush;

            Button btn = new Button();
            btn.Content = textBox;
            btn.Click += LoadNewVideo;
            btn.DataContext = id.ToString();
            btn.HorizontalAlignment = HorizontalAlignment.Stretch;
            btn.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            btn.Height = 50;
            btn.BorderThickness = borderNone;

            if(v.Complete)
            {
                btn.Background = videoCompletedBrush;
                textBox.Foreground = textBoxCompletedBrush;
            }
            else
            {
                btn.Background = videoBrush;
            }
            videoPanel.Children.Add(btn);

            ProgressBar bar = new ProgressBar();
            bar.Height = 20;

            if (v.Duration == 0)
                bar.Maximum = 100;
            else
                bar.Maximum = v.Duration;

            bar.Value = v.CurrentTime;
            bar.BorderThickness = borderNone;
            bar.DataContext = id.ToString();
            bar.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            bar.HorizontalAlignment = HorizontalAlignment.Stretch;
            bar.Margin = onlyBottomMargin;
            bar.Background = barBackBrush;
            bar.Foreground = orangeBrush;

            videoPanel.Children.Add(bar);
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
            if(player.currentVideo != null)
            {
                SaveVideoData(null, null);
                player.currentVideo.Completed -= OnVideoComplete;
            }

            if (curPressedButton != null && !player.currentVideo.Complete)
                curPressedButton.Background = videoBrush;

            player.Play(videos.ElementAt(int.Parse((sender as Button).DataContext.ToString())));

            btnPlay.Background = pauseImageBrush;

            curPressedButton = (sender as Button);
            player.currentVideo.Completed += OnVideoComplete;
            player.SetTime(player.currentVideo.CurrentTime);

            if (!player.currentVideo.Complete)
                curPressedButton.Background = activeVideoBrush;
            player.mPlayer.Playing += LoadSubtitles;

            curProgressBar = (ProgressBar)VisualTreeHelper.GetChild(videoPanel, int.Parse(curPressedButton.DataContext.ToString()) * 2 + 1);
        }

        private void Move(object sender, RoutedEventArgs e)
        {
            if (player.currentMedia == null || !player.mPlayer.IsPlaying)
                return;

            int value = int.Parse((sender as Button).Tag.ToString());

            if(value > 0) // right
            {
                player.MoveBy(3000);
            }
            else
            {
                // left
                player.MoveBy(-3000);
            }
        }

        private void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            player = new VideoPlayer();
            videoView.MediaPlayer = player.mPlayer;
            player.mPlayer.TimeChanged += Update;

            // making sure user can't
            // start playing video until video view is loaded
            CreateVideoListings();
        }

        private void GoToHomePage(object sender, RoutedEventArgs e)
        {
            // save current video data, if it was loaded
            if (player.mPlayer != null)
                DataManager.UpdateVideoData(categoryName, player.GetVideo());

            player.Pause();
            player.Dispose();
            mainWindow.Title = "Video Tracker - Home";
            NavigationService.GoBack();
        }

        private void Update(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            sliderTimeline.Dispatcher.InvokeAsync(() =>
            {
                if (sliderTimeline.Maximum != Convert.ToDouble(player.currentMedia.Duration / 60))
                {
                    sliderTimeline.Maximum = Convert.ToDouble(player.currentMedia.Duration / 60);
                    player.currentVideo.Duration = Convert.ToUInt32(player.currentMedia.Duration / 60);
                    // making sure volume slider is on some kind of value
                    sliderVolume.Value = 50; // default
                }

                sliderTimeline.Value = Convert.ToDouble(e.Time / 60);

                if (curProgressBar.Maximum == 100)
                    curProgressBar.Maximum = player.currentVideo.Duration;

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

        private void ShowSubtitlesContextMenu(object sender, RoutedEventArgs e)
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
                btnPlay.Background = playImageBrush;
            }
            else
            {
                player.Play();
                btnPlay.Background = pauseImageBrush;
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

        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Left:
                    player.MoveBy(-3000);
                    break;
                
                case Key.Right:
                    player.MoveBy(3000);
                    break;
                // pause/play video
                case Key.Space:
                    PlayVideo(null, null);
                    break;
                // exit fullscreen
                case Key.Escape:
                    if(isFullscreen)
                    {
                        EnterFullscreenMode();
                    }
                    break;
                // enter fullscreen
                case Key.F:
                    if(!isFullscreen)
                    {
                        EnterFullscreenMode();
                    }
                    break;

                case Key.Add:
                case Key.OemPlus:
                    if (sliderVolume.Value + 10 > 100)
                        sliderVolume.Value = 100;
                    else
                        sliderVolume.Value += 10;
                    break;

                case Key.Subtract:
                case Key.OemMinus:
                    if (sliderVolume.Value - 10 < 0)
                        sliderVolume.Value = 0;
                    else
                        sliderVolume.Value -= 10;
                    break;

                default:
                    break;
            }
        }

        private void SelectSubtitleItem(object sender, RoutedEventArgs e)
        {
            MenuItem test = sender as MenuItem;
            player.mPlayer.SetSpu(int.Parse(test.DataContext.ToString()));
        }

        private void LoadSubtitles(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                // remove items from previous loaded subtitles
                videoViewContextMenu.Items.Clear();
                MenuItem addSub = new MenuItem();
                addSub.Header = "Add subtitle file...";
                addSub.Click += AddExternalSubFile;
                videoViewContextMenu.Items.Add(addSub);

                foreach(TrackDescription t in player.mPlayer.SpuDescription)
                {
                    MenuItem sub = new MenuItem();
                    sub.Header = t.Name;
                    sub.DataContext = t.Id;
                    sub.Click += SelectSubtitleItem;
                    videoViewContextMenu.Items.Add(sub);
                }
            });
            player.mPlayer.Playing -= LoadSubtitles;
        }

        private void AddExternalSubFile(object sender, EventArgs e)
        {
            if(player.currentMedia != null)
            {
                player.Pause();

                CommonOpenFileDialog openDialog = new CommonOpenFileDialog();
                if (openDialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    player.AddSub(openDialog.FileName);
                }
                player.Play();
            }
        }
    }
}
