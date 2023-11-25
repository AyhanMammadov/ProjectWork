﻿using System;
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
            const string address = "http://localhost:8080/cars/add";
            HttpClient httpClient = new HttpClient();

            var response = await httpClient.GetAsync(address);
            var responseTxt = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(responseTxt))
            {
                var result = JsonSerializer.Deserialize<IEnumerable<Car>>(responseTxt);

                foreach (var car in result)
                {
                    this.CarsListView.Items.Add(car);
                }
            }
            else
            {
                MessageBox.Show("Server returned empty or invalid JSON data.");
            }
        }



        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddWindow createCar = new AddWindow();
            
            createCar.ShowDialog();
            createCar.Close();

        }
    }
}
