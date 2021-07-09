using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace video_tracker_v2
{
    public static class UI
    {
        public static Thickness BorderThickness { get; private set; }
        public static Thickness Margin { get; private set; }
        public static SolidColorBrush TextBoxBrush { get; private set; }
        public static SolidColorBrush CategoryNormalBrush { get; private set; }
        public static SolidColorBrush ButtonNormalBrush { get; private set; }
        public static SolidColorBrush ButtonActiveBrush { get; private set; }
        public static SolidColorBrush BorderBrush { get; private set; }
        public static int CategoryWidth { get; private set; }
        public static int CategoryHeight { get; private set; }
        public static int FontSize { get; private set; }

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
    }
}
