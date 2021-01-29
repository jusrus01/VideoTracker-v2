using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace video_tracker_v2
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private string[] categories;
        private bool deleting = false;

        // dynamic elements style values
        private Thickness borderThickness;
        private Thickness margin;
        private SolidColorBrush textBoxBrush;
        private SolidColorBrush categoryNormalBrush;
        private SolidColorBrush buttonNormalBrush;
        private SolidColorBrush buttonActiveBrush;
        private SolidColorBrush borderBrush;

        private readonly int categoryWidth = 254;
        private readonly int categoryHeight = 146;
        private readonly int fontSize = 26;

        public HomePage()
        {
            InitializeComponent();

            DataManager.MainPath = "categories.data";
            DataManager.CreateDataFolder();

            // init style values
            borderThickness = new Thickness(0);
            textBoxBrush = new SolidColorBrush(Color.FromRgb(249, 250, 249));
            categoryNormalBrush = new SolidColorBrush(Color.FromRgb(114, 117, 121));
            buttonNormalBrush = new SolidColorBrush(Color.FromRgb(131, 162, 167));
            buttonActiveBrush = new SolidColorBrush(Color.FromRgb(171, 202, 207));
            borderBrush = new SolidColorBrush(Color.FromRgb(39, 32, 42));
            margin = new Thickness(5, 10, 5, 0);

            CreateCategories();
        }

        private void CreateCategories()
        {
            categories = DataManager.LoadCategories();

            if (categories == null)
                return;

            foreach (string path in categories)
            {
                CreateButton(path);
            }
        }

        private void CreateButton(string path)
        {
            TextBox textBox = new TextBox();

            textBox.Text = System.IO.Path.GetFileName(path);
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.HorizontalContentAlignment = HorizontalAlignment.Center;
            textBox.FontSize = fontSize;
            textBox.Focusable = false;
            textBox.Background = Brushes.Transparent;
            textBox.BorderThickness = borderThickness;
            textBox.Cursor = Cursors.Arrow;
            textBox.Foreground = textBoxBrush;

            Button btn = new Button();

            btn.Click += Button_Click;
            btn.DataContext = path;
            btn.Width = categoryWidth;
            btn.Height = categoryHeight;
            btn.Background = categoryNormalBrush;
            btn.VerticalContentAlignment = VerticalAlignment.Center;
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            btn.BorderBrush = borderBrush;
            btn.Content = textBox;
            btn.Margin = margin;

            panelCategories.Children.Add(btn);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (deleting)
            {
                // delete button and remove from file
                Button btn = (sender as Button);
                DataManager.RemoveCategoryFromFile(btn.DataContext.ToString());
                panelCategories.Children.Remove(btn);
                ToogleRemove(null, null);
            }
            else
            {
                VideosPage videosPage = new VideosPage((sender as Button).DataContext as string);
                this.NavigationService.Navigate(videosPage);
            }
        }

        private void ToogleRemove(object sender, RoutedEventArgs e)
        {
            if (deleting)
            {
                btnRemove.Background = buttonNormalBrush;
                deleting = false;
            }
            else
            {
                deleting = true;
                btnRemove.Background = buttonActiveBrush;
            }
        }

        private void AddCategory(object sender, RoutedEventArgs e)
        {
            if(deleting)
                ToogleRemove(null, null);

            CommonOpenFileDialog openDialog = new CommonOpenFileDialog();
            openDialog.IsFolderPicker = true;

            if (openDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                // check to see if it already exists
                if (!DataManager.EntryExists(openDialog.FileName))
                {
                    CreateButton(openDialog.FileName);
                    DataManager.SaveCategory(openDialog.FileName);
                }
            }
        }
    }
}
