using Microsoft.Win32;
using SharedLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for Update.xaml
    /// </summary>
    public partial class Update : Window
    {

        private readonly HttpClient httpClient;

        public int id = 0;
        public Update()
        {
            InitializeComponent();
            this.httpClient = new HttpClient();
        }

        public Update(int id) : this()
        {
            InitializeComponent();
            this.id = id;
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

         

            var response = await httpClient.GetAsync(address);
            var responseTxt = await response.Content.ReadAsStringAsync();


            var result = JsonSerializer.Deserialize<IEnumerable<Car>>(responseTxt);

            var carFindById = result.Where(c => c.Id == this.id).First();

            if (string.IsNullOrWhiteSpace(this.ModelTextBox.Text) == false && string.IsNullOrWhiteSpace(this.DescriptionTextBox.Text) == false
                && string.IsNullOrWhiteSpace(this.AddingImage.Source.ToString()) == false)
            {
                carFindById.Model = this.ModelTextBox.Text;
                carFindById.Description = this.DescriptionTextBox.Text;
                carFindById.PathImage = this.AddingImage.Source.ToString();
                
            }
            else
            {
                MessageBox.Show("Fields Can not be empty");
            }

            var jsonCar = JsonSerializer.Serialize(carFindById);
            var content = new StringContent(jsonCar, Encoding.UTF8, "application/json");

            var putMethod = await httpClient.PutAsync(address+"/update", content);


            var responseTXT = await response.Content.ReadAsStringAsync();
            
            this.Close();

        }
    }
}
