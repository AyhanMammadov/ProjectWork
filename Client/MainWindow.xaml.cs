using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using SharedLib.Models;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Cars_Loaded();
            this.DescriptionTextBox.IsEnabled = false;

        }

        private async void Cars_Loaded()
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
            MessageBox.Show(responseTxt);
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
    }
}
