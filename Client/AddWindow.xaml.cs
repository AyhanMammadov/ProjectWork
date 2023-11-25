using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using SharedLib.Models;

namespace Client
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage image = new BitmapImage(new Uri(openFileDialog.FileName));
                    this.AddingImage.Source = image;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            const string address = "http://localhost:8080/cars";
            HttpClient httpClient = new HttpClient();
            var newUserJson = new Car()
            {
                Model = this.ModelTextBox.Text,
                Description = DescriptionTextBox.Text,
                PathImage = this.AddingImage.Source.ToString(),
            };
            var content = JsonContent.Create(newUserJson);
            var response = await httpClient.PostAsync(address, content);
            var responseTxt = await response.Content.ReadAsStringAsync();

            //var bitmapImage = new BitmapImage();
            //this.photoImage.Source = bitmapImage.ChangePic(this.blog.PathImage);


            //var content = JsonContent.Create(newUserJson);
            //var response = await httpClient.PostAsync(address, content);
            //var responseTxt = await response.Content.ReadAsStringAsync();

            //System.Console.WriteLine($"Content: {responseTxt}");
            //System.Console.WriteLine(response);
        }
    }
}
