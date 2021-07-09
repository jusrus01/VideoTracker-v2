using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace video_tracker_v2
{
    public static class UI
    {
        // style values for dynamic button creation
        private static Thickness borderThickness = new Thickness(0);
        private static Thickness margin = new Thickness(5, 10, 5, 0);
        private static SolidColorBrush textBoxBrush = new SolidColorBrush(Color.FromRgb(235, 232, 222));
        private static SolidColorBrush categoryNormalBrush = new SolidColorBrush(Color.FromRgb(64, 61, 57));
        public static SolidColorBrush buttonNormalBrush = new SolidColorBrush(Color.FromRgb(235, 94, 40));
        public static SolidColorBrush buttonActiveBrush = new SolidColorBrush(Color.FromRgb(245, 134, 80));
        private static SolidColorBrush borderBrush = new SolidColorBrush(Color.FromRgb(37, 36, 34));
        private static readonly int categoryWidth = 254;
        private static readonly int categoryHeight = 146;
        private static readonly int fontSize = 20;

        public static Button CreateButton(string label)
        {
            TextBox textBox = new TextBox();

            textBox.Text = label;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.FontSize = fontSize;
            textBox.Focusable = false;
            textBox.Background = Brushes.Transparent;
            textBox.BorderThickness = borderThickness;
            textBox.Cursor = Cursors.Arrow;
            textBox.Foreground = textBoxBrush;

            Button btn = new Button();

            btn.Width = categoryWidth;
            btn.Height = categoryHeight;
            btn.Background = categoryNormalBrush;
            btn.VerticalContentAlignment = VerticalAlignment.Center;
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.BorderBrush = borderBrush;
            btn.Content = textBox;
            btn.Margin = margin;

            return btn;
        }
    }
}
