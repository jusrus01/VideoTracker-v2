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

namespace video_tracker_v2
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();

            DataManager.Path = @"..\..\categories.data";

        }

        private void CreateCategories()
        {
            string[] categories = DataManager.LoadCategories();
            if (categories == null)
                return;

            foreach(string path in categories)
            {
                // create button
                CreateButton(path);
            }
        }

        // create from application and save data
        private void CreateButton(object sender, RoutedEventArgs e)
        {

        }

        // create from data file 
        private void CreateButton(string path)
        {
            Button btn = new Button();
            btn.Content = "TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST";
            btn.Name = path;
            panelCategories.Children.Add(btn);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VideosPage videosPage = new VideosPage((sender as Button).Name);
            this.NavigationService.Navigate(videosPage);
        }

        private void AddCategory(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog openDialog = new CommonOpenFileDialog();
            openDialog.IsFolderPicker = true;

            if (openDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                CreateButton(null);
            }
        }
    }
}
