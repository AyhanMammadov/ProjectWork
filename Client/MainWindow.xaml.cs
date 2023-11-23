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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        }

        private async void Cars_Loaded()
        {
            const string address = "http://localhost:8080/cars";
            HttpClient httpClient = new HttpClient();

            var response = await httpClient.GetAsync(address);
            var responseTxt = await response.Content.ReadAsStringAsync();

            
        }
    }
}
