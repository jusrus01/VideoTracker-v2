using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace video_tracker_v2
{
    public static class UI
    {
        public static Thickness BorderThickness { get; }
        public static Thickness Margin { get; }
        public static SolidColorBrush TextBoxBrush { get;  }
        public static SolidColorBrush CategoryNormalBrush { get; }
        public static SolidColorBrush ButtonNormalBrush { get; }
        public static SolidColorBrush ButtonActiveBrush { get; }
        public static SolidColorBrush BorderBrush { get; }
        public static int CategoryWidth { get; }
        public static int CategoryHeight { get; }
        public static int FontSize { get; }
        public static SolidColorBrush VideoCompletedBrush { get; }
        public static SolidColorBrush TextBoxCompletedBrush { get; }
        public static SolidColorBrush VideoBrush { get; }
        public static SolidColorBrush OrangeBrush { get; }
        public static SolidColorBrush ActiveVideoBrush { get; }
        public static SolidColorBrush BarBackBrush { get; }
        public static Thickness BorderNone { get; }
        public static Thickness OnlyBottomMargin { get; }
        public static ImageBrush PlayImageBrush { get; }
        public static ImageBrush PauseImageBrush { get; }

        static UI()
        {
            BorderThickness = new Thickness(0);
            Margin = new Thickness(5, 10, 5, 0);
            TextBoxBrush = new SolidColorBrush(Color.FromRgb(235, 232, 222));
            CategoryNormalBrush = new SolidColorBrush(Color.FromRgb(64, 61, 57));
            ButtonNormalBrush = new SolidColorBrush(Color.FromRgb(235, 94, 40));
            ButtonActiveBrush = new SolidColorBrush(Color.FromRgb(245, 134, 80));
            BorderBrush = new SolidColorBrush(Color.FromRgb(37, 36, 34));
            CategoryWidth = 254;
            CategoryHeight = 146;
            FontSize = 20;
            VideoCompletedBrush = new SolidColorBrush(Color.FromRgb(235, 94, 40));
            TextBoxCompletedBrush = new SolidColorBrush(Color.FromRgb(255, 215, 162));
            VideoBrush = new SolidColorBrush(Color.FromRgb(64, 61, 57));
            OrangeBrush = new SolidColorBrush(Color.FromRgb(235, 94, 40));
            ActiveVideoBrush = new SolidColorBrush(Color.FromRgb(104, 101, 97));
            BarBackBrush = new SolidColorBrush(Color.FromRgb(44, 41, 37));
            BorderNone = new Thickness(0);
            OnlyBottomMargin = new Thickness(0, 0, 0, 5);
            PlayImageBrush = new ImageBrush(new BitmapImage(new Uri(@"..\..\Video Tracker\Resources\play-button.png", UriKind.Relative)));
            PauseImageBrush = new ImageBrush(new BitmapImage(new Uri(@"..\..\Video Tracker\Resources\pause-button.png", UriKind.Relative)));
        }

        public static Button CreateButton(string label)
        {
            TextBox textBox = new TextBox();

            textBox.Text = label;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.FontSize = FontSize;
            textBox.Focusable = false;
            textBox.Background = Brushes.Transparent;
            textBox.BorderThickness = BorderThickness;
            textBox.Cursor = Cursors.Arrow;
            textBox.Foreground = TextBoxBrush;

            Button btn = new Button();

            btn.Width = CategoryWidth;
            btn.Height = CategoryHeight;
            btn.Background = CategoryNormalBrush;
            btn.VerticalContentAlignment = VerticalAlignment.Center;
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.BorderBrush = BorderBrush;
            btn.Content = textBox;
            btn.Margin = Margin;

            return btn;
        }

        public static Button CreateButton(Video video, int id)
        {
            TextBox textBox = new TextBox();

            textBox.Text = System.IO.Path.GetFileNameWithoutExtension(video.Path);
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.HorizontalContentAlignment = HorizontalAlignment.Left;
            textBox.FontSize = FontSize;
            textBox.Focusable = false;
            textBox.Background = Brushes.Transparent;
            textBox.BorderThickness = BorderNone;
            textBox.Cursor = Cursors.Arrow;
            textBox.Foreground = TextBoxBrush;

            Button btn = new Button();
            btn.Content = textBox;
            btn.DataContext = id.ToString();
            btn.HorizontalAlignment = HorizontalAlignment.Stretch;
            btn.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            btn.Height = 50;
            btn.BorderThickness = BorderNone;

            if (video.Complete)
            {
                btn.Background = VideoCompletedBrush;
                textBox.Foreground = TextBoxCompletedBrush;
            }
            else
            {
                btn.Background = VideoBrush;
            }
            return btn;

        }

        public static ProgressBar CreateProgressBar(Video video, int id)
        {

            ProgressBar bar = new ProgressBar();
            bar.Height = 20;

            if (video.Duration == 0)
                bar.Maximum = 100;
            else
                bar.Maximum = video.Duration;

            bar.Value = video.CurrentTime;
            bar.BorderThickness = BorderNone;
            bar.DataContext = id.ToString();
            bar.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            bar.HorizontalAlignment = HorizontalAlignment.Stretch;
            bar.Margin = OnlyBottomMargin;
            bar.Background = BarBackBrush;
            bar.Foreground = OrangeBrush;

            return bar;
        }
    }
}
