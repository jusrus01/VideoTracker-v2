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
using Microsoft.WindowsAPICodePack.Dialogs;

// TO DO: remove duplicate code
// TO DO: Fix code
// TO DO: handle complete
// fix progress bar
// implement color change on video completion

namespace video_tracker_v2
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private string[] categories;
        private bool deleting = false;

        public HomePage()
        {
            InitializeComponent();

            DataManager.MainPath = "categories.data";
            DataManager.CreateDataFolder();

            CreateCategories();
        }

        private void CreateCategories()
        {
            categories = DataManager.LoadCategories();
            if (categories == null)
                return;

            foreach(string path in categories)
            {
                CreateButton(path);
            }
        }

        private void CreateButton(string path)
        {
            TextBox textBox = new TextBox();
            textBox.Text = System.IO.Path.GetFileName(path);
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.FontSize = 26;
            textBox.Focusable = false;
            textBox.Background = Brushes.Transparent;
            textBox.BorderThickness = new Thickness(0);
            textBox.Cursor = Cursors.Arrow;
            textBox.Foreground = new SolidColorBrush(Color.FromRgb(249, 250, 249));

            Button btn = new Button();
            btn.Click += Button_Click;
            btn.DataContext = path;
            btn.Width = 254;
            btn.Height = 146;
            btn.Background = new SolidColorBrush(Color.FromRgb(114, 117, 121));
            btn.VerticalContentAlignment = VerticalAlignment.Center;
            btn.HorizontalAlignment = HorizontalAlignment.Center;
            //btn.FontSize = 26;
            //btn.BorderThickness = new Thickness(0);
            btn.BorderBrush = new SolidColorBrush(Color.FromRgb(39, 32, 42));
            //btn.Content = System.IO.Path.GetFileName(path);
            btn.Content = textBox;
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
                deleting = false;
                btnRemove.Background = new SolidColorBrush(Color.FromRgb(131, 162, 167));
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
                btnRemove.Background = new SolidColorBrush(Color.FromRgb(131, 162, 167));
                deleting = false;
            }
            else
            {
                deleting = true;
                btnRemove.Background = new SolidColorBrush(Color.FromRgb(171, 202, 207));
            }
        }

        private void AddCategory(object sender, RoutedEventArgs e)
        {
            if(deleting)
            {
                deleting = false;
                btnRemove.Background = new SolidColorBrush(Color.FromRgb(131, 162, 167));
            }

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
