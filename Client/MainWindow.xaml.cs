using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
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
using Client.ChangePicture;
using Microsoft.Win32;
using SharedLib.Models;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MediaPlayer mediaPlayer;
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer = new MediaPlayer();


            mediaPlayer.Open(new Uri("Assets/SnoopDog.mp3", UriKind.Relative));



            mediaPlayer.Play();
            Cars_Loaded();
            this.DescriptionTextBox.IsEnabled = false;

        }

        private async void Cars_Loaded()
        {
            try
            {
                this.CarsListView.Items.Clear();
                const string address = "http://localhost:8080/cars";
                HttpClient httpClient = new HttpClient();

                var response = await httpClient.GetAsync(address);
                var responseTxt = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<IEnumerable<Car>>(responseTxt);

                foreach (var VARIABLE in result)
                {
                    this.CarsListView.Items.Add(VARIABLE);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            
        }



        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddWindow createCar = new AddWindow();
            
            createCar.ShowDialog();
            createCar.Close();
            Cars_Loaded();

        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            const string address = "http://localhost:8080/cars/delete/";
            HttpClient httpClient = new HttpClient();

            var res = this.CarsListView.SelectedItem as Car;

            var response = await httpClient.DeleteAsync($"{address}{res.Id}");
            var responseTxt = await response.Content.ReadAsStringAsync();
            MessageBox.Show("Deleted");
            var bitmapImage = new BitmapImage();
            this.Image.Source = bitmapImage.ChangePic("/Assets/Logo.png");
            this.DescriptionTextBox.Text = string.Empty;
            Cars_Loaded();
        }


        private async void Update_Click(object sender, RoutedEventArgs e)
        {

            var res = this.CarsListView.SelectedItem as Car;

            Update update = new Update(res.Id);
            update.ShowDialog();
            update.Close();
            Cars_Loaded();
        }

        private void CarsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var bitmapImage = new BitmapImage();
            var selectedItem = this.CarsListView.SelectedItem as Car;
            if (selectedItem != null)
            {
                this.DescriptionTextBox.Text = selectedItem.Description;
                this.Image.Source = bitmapImage.ChangePic(selectedItem.PathImage);
            }

            
        }
    }
}
