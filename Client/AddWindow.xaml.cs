using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
        private readonly HttpClient httpClient;

        public AddWindow()
        {
            InitializeComponent();
            this.httpClient = new HttpClient();
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
            try
            {
                const string address = "http://localhost:8080/cars/add";

                if (string.IsNullOrWhiteSpace(this.ModelTextBox.Text) == false && string.IsNullOrWhiteSpace(this.DescriptionTextBox.Text) == false
                                                                               && string.IsNullOrWhiteSpace(this.AddingImage.Source.ToString()) == false)
                {

                    var newCar = new Car()
                    {
                        Model = this.ModelTextBox.Text,
                        Description = DescriptionTextBox.Text,
                        PathImage = this.AddingImage.Source.ToString(),
                    };

                    var jsonCar = JsonSerializer.Serialize(newCar);
                    var content = new StringContent(jsonCar, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(address, content);
                    var responseTxt = await response.Content.ReadAsStringAsync();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Fields can not be empty");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
            
        }
    }
}
