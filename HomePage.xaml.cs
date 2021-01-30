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

        // style values for dynamic button creation
        private Thickness borderThickness;
        private Thickness margin;
        private SolidColorBrush textBoxBrush;
        private SolidColorBrush categoryNormalBrush;
        private SolidColorBrush buttonNormalBrush;
        private SolidColorBrush buttonActiveBrush;
        private SolidColorBrush borderBrush;

        private readonly int categoryWidth = 254;
        private readonly int categoryHeight = 146;
        private readonly int fontSize = 20;

        public HomePage()
        {
            InitializeComponent();

            DataManager.MainPath = "categories.data";
            DataManager.CreateDataFolder();

            // init style values
            borderThickness = new Thickness(0);
            textBoxBrush = new SolidColorBrush(Color.FromRgb(235, 232, 222));
            categoryNormalBrush = new SolidColorBrush(Color.FromRgb(64, 61, 57));
            buttonNormalBrush = new SolidColorBrush(Color.FromRgb(235, 94, 40));
            buttonActiveBrush = new SolidColorBrush(Color.FromRgb(245, 134, 80));
            borderBrush = new SolidColorBrush(Color.FromRgb(37, 36, 34));
            margin = new Thickness(5, 10, 5, 0);

            CreateCategories();
        }

        /// <summary>
        /// Create buttons for each path saved in
        /// data file
        /// </summary>
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

        /// <summary>
        /// Create button with associated path
        /// to video directory
        /// </summary>
        /// <param name="path">Path to video directory</param>
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

        /// <summary>
        /// Handles page transition with button click
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event data</param>
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

        /// <summary>
        /// Sets state to deletion
        /// and changes button color
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event data</param>
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

        /// <summary>
        /// Opens file dialog and allows user to
        /// select path to videos directory
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event data</param>
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
