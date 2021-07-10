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
using System.Threading.Tasks;
using System.IO;

namespace video_tracker_v2
{
    /// <summary>
    /// Interaction logic for VideosPage.xaml
    /// </summary>
    public partial class VideosPage : Page
    {
        private string categoryName;
        private string currentCategoryPath;

        private Window mainWindow;
        private bool isFullscreen = false;

        private Timer timer;

        private VideoPlayer player;
        private List<Video> videos;

        private Button curPressedButton;
        private ProgressBar curProgressBar;

        public VideosPage(string path)
        {
            InitializeComponent();

            currentCategoryPath = path;
            categoryName = Path.GetFileName(path);

            mainWindow = Application.Current.MainWindow;
            Application.Current.Exit += SaveVideoData;
            mainWindow.Title = "Video Tracker - " + categoryName;

            videoView.Loaded += VideoView_Loaded;

            timer = new Timer(3000);
            timer.Elapsed += HideCursor;

            // subscribe to key up event
            EventManager.RegisterClassHandler(typeof(Window),
                Keyboard.PreviewKeyUpEvent, new KeyEventHandler(HandleKeyUp), true);
        }

        private void CreateVideoListings()
        {
            for(int i = 0; i < videos.Count; i++)
            {
                AddButton(videos[i], i);
                AddProgressBar(videos[i], i);
            }
        }

        private void OnVideoComplete()
        {
            curPressedButton.Background = UI.VideoCompletedBrush;
            ((TextBox)curPressedButton.Content).Foreground = UI.TextBoxCompletedBrush;
        }

        private void AddButton(Video v, int id)
        {
            Button button = UI.CreateButton(v, id);
            button.Click += LoadNewVideo;

            videoPanel.Children.Add(button);
        }

        private void AddProgressBar(Video v, int id)
        {
            ProgressBar bar = UI.CreateProgressBar(v, id);

            videoPanel.Children.Add(bar);
        }

        private void InsertToVideoList(Video video)
        {
            video.CurrentTime = player.Time;
            video.Duration = player.VideoDuration;

            if(videos != null && video != null)
            {
                for(int i = 0; i < videos.Count; i++)
                {
                    if(videos[i].Equals(video))
                    {
                        videos[i] = video;
                        return;
                    }
                }
            }
        }

        private void SaveVideoData(object sender, ExitEventArgs e)
        {
            if (player.currentVideo != null)
            {
                //// save everything on exit
                //player.currentVideo.CurrentTime = player.Time;
                //player.currentVideo.Duration = player.VideoDuration;
                InsertToVideoList(player.currentVideo);
                DataManager.UpdateVideosDataAsync(categoryName, videos);
            }
        }

        private void LoadNewVideo(object sender, RoutedEventArgs e)
        {
            if(player.currentVideo != null)
            {
                player.currentVideo.Completed -= OnVideoComplete;
                InsertToVideoList(player.currentVideo);
            }

            if (curPressedButton != null && !player.currentVideo.Complete)
                curPressedButton.Background = UI.VideoBrush;

            player.Play(videos.ElementAt(int.Parse((sender as Button).DataContext.ToString())));

            btnPlay.Background = UI.PauseImageBrush;

            curPressedButton = (sender as Button);
            player.currentVideo.Completed += OnVideoComplete;
            player.SetTime(player.currentVideo.CurrentTime);

            if (!player.currentVideo.Complete)
                curPressedButton.Background = UI.ActiveVideoBrush;

            player.AddCallbackOnMediaPlaying(LoadSubtitles);

            curProgressBar = (ProgressBar)VisualTreeHelper.GetChild(videoPanel, int.Parse(curPressedButton.DataContext.ToString()) * 2 + 1);
        }

        private void Move(object sender, RoutedEventArgs e)
        {
            if (!player.IsPlaying)
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

        private async void VideoView_Loaded(object sender, RoutedEventArgs e)
        {
            player = new VideoPlayer();
            player.AddCallbackOnTimeChanged(Update);

            videoView.MediaPlayer = player.GetMediaPlayer();

            videos = await DataManager.LoadVideosAsync(categoryName);
            // making sure user can't
            // start playing video until video view is loaded
            CreateVideoListings();
            
        }

        private void GoToHomePage(object sender, RoutedEventArgs e)
        {
            InsertToVideoList(player.currentVideo);
            DataManager.UpdateVideosDataAsync(categoryName, videos);

            player.Pause();
            player.Dispose();
            mainWindow.Title = "Video Tracker - Home";

            NavigationService.GoBack();
        }

        private void Update(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                double watchedTime = Math.Round(player.Time * sliderTimeline.Maximum / player.VideoDuration);

                sliderTimeline.Value = watchedTime;
                curProgressBar.Value = watchedTime;
            });

            // async change of colors
            // async change of decorative progress bar

            // real time change of slider

            //sliderTimeline.Dispatcher.InvokeAsync(() =>
            //{
            //    // maximum - 100
            //    // x - y
            //    sliderTimeline.Value = Math.Round(e.Time * sliderTimeline.Maximum / player.VideoDuration);

            //});

            //sliderTimeline.Dispatcher.InvokeAsync(() =>
            //{
            //    if (sliderTimeline.Maximum != Convert.ToDouble(player.VideoDuration))
            //    {
            //        sliderTimeline.Maximum = Convert.ToDouble(player.VideoDuration);
            //        player.currentVideo.Duration = player.VideoDuration;
            //        // making sure volume slider is on some kind of value
            //        sliderVolume.Value = 50; // default
            //    }

            //    sliderTimeline.Value = e.Time;

            //    if (curProgressBar.Maximum == 100)
            //        curProgressBar.Maximum = player.currentVideo.Duration;

            //    curProgressBar.Value = e.Time;

            //    // updating video file too
            //    player.currentVideo.CurrentTime = e.Time;
            //});
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


        private void SliderPositionChange(object sender, MouseButtonEventArgs e)
        {
            player.SetTime((long)sliderTimeline.Value * player.VideoDuration / (long)sliderTimeline.Maximum);
        }

        private void EnterFullscreenMode()
        {
            player.Pause();
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
            // this seems to decrease chance of app crash
            Task.Delay(1000).ContinueWith(t => player.Play());
        }

        private void PlayVideo(object sender, RoutedEventArgs e)
        {
            if(player.IsPlaying)
            {
                player.Pause();
                btnPlay.Background = UI.PlayImageBrush;
            }
            else
            {
                player.Play();
                btnPlay.Background = UI.PauseImageBrush;
            }
        }

        private void SetVideoToTime(object sender, RoutedEventArgs e)
        {
            // check if it's really a mouse click
            //Slider s = sender as Slider;
            //if (Convert.ToDouble(player.Time) != s.Value)
            //{
            //    player.SetTime((long)s.Value);
            //}
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
            player.SetSpu(int.Parse(test.DataContext.ToString()));
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

                foreach(TrackDescription t in player.SpuDescription)
                {
                    MenuItem sub = new MenuItem();
                    sub.Header = t.Name;
                    sub.DataContext = t.Id;
                    sub.Click += SelectSubtitleItem;
                    videoViewContextMenu.Items.Add(sub);
                }
            });

            player.RemoveCallbackOnMediaPlaying(LoadSubtitles);
        }

        private void AddExternalSubFile(object sender, EventArgs e)
        {
            if(player.MediaLoaded)
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
