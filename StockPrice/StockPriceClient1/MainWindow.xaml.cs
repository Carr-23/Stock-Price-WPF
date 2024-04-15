using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

namespace StockPriceClient1
{
    [ServiceContract]
    public interface IStockPriceService
    {
        [OperationContract]
        double getLatestPrice(string stockName);

        [OperationContract]
        int Test();
    }

    public partial class MainWindow : Window
    {
        // Represents the proxy for the WCF service
        private IStockPriceService proxy;
        // Stores historical stock prices
        private Dictionary<string, List<double>> stockPriceHistory = new Dictionary<string, List<double>>();
        // Stores timing information for price updates
        private Dictionary<string, List<string>> timing = new Dictionary<string, List<string>>();

        // Flag to subscribe to stock updates
        private bool stock1Sub = true;
        private bool stock2Sub = true;

        public MainWindow()
        {
            InitializeComponent();
            // Initializes the WCF service proxy
            InitializeService();

            // Initialize data structures for stock price history and timing
            stockPriceHistory["Stock1"] = new List<double>();
            stockPriceHistory["Stock2"] = new List<double>();
            timing["Stock1"] = new List<string>();
            timing["Stock2"] = new List<string>();

            // Starts the process to get price updates
            GetPriceUpdates();
        }

        // Event handlers for checkbox changes
        private void stock1Checked(object sender, RoutedEventArgs e)
        {
            stock1Sub = true;
        }

        private void stock1Unchecked(object sender, RoutedEventArgs e)
        {
            stock1Sub = false;
            Button button = (Button)grid.FindName("buttonStock1");
            button.Background = Brushes.DarkGray;
        }

        private void stock2Checked(object sender, RoutedEventArgs e)
        {
            stock2Sub = true;
        }

        private void stock2Unchecked(object sender, RoutedEventArgs e)
        {
            stock2Sub = false;
            Button button = (Button)grid.FindName("buttonStock2");
            button.Background = Brushes.DarkGray;
        }

        // Event handlers for button clicks to view stock history
        private void stock1Click(object sender, RoutedEventArgs e)
        {
            History detailsWindow = new History(stockPriceHistory["Stock1"], timing["Stock1"]);
            detailsWindow.Show();
        }

        private void stock2Click(object sender, RoutedEventArgs e)
        {
            History detailsWindow = new History(stockPriceHistory["Stock2"], timing["Stock2"]);
            detailsWindow.Show();
        }

        // Async method to get prices
        private void GetPriceUpdates()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (stock1Sub)
                    {
                        getLatestPrice("Stock1");
                    }

                    if (stock2Sub)
                    {
                        getLatestPrice("Stock2");
                    }
                    // Updates UI based on price changes
                    updateBackgroundColour();
                    // Delay between updates
                    await Task.Delay(1000);
                }
            });
        }

        // Update buttons based on previous value
        private void updateBackgroundColour()
        {
            Dispatcher.Invoke(() =>
            {
                if (stock1Sub && stockPriceHistory["Stock1"].Count > 1)
                {
                    double lastPrice1 = stockPriceHistory["Stock1"][stockPriceHistory["Stock1"].Count - 1];
                    double prevPrice1 = stockPriceHistory["Stock1"][stockPriceHistory["Stock1"].Count - 2];

                    Button button1 = (Button)grid.FindName("buttonStock1");

                    if (lastPrice1 > prevPrice1)
                    {
                        button1.Background = Brushes.LightGreen;
                    }
                    else if (lastPrice1 < prevPrice1)
                    {
                        button1.Background = Brushes.Red;
                    }
                    else
                    {
                        button1.Background = Brushes.White;
                    }
                }

                if (stock2Sub && stockPriceHistory["Stock2"].Count > 1)
                {
                    double lastPrice2 = stockPriceHistory["Stock2"][stockPriceHistory["Stock2"].Count - 1];
                    double prevPrice2 = stockPriceHistory["Stock2"][stockPriceHistory["Stock2"].Count - 2];

                    Button button2 = (Button)grid.FindName("buttonStock2");

                    if (lastPrice2 > prevPrice2)
                    {
                        button2.Background = Brushes.LightGreen;
                    }
                    else if (lastPrice2 < prevPrice2)
                    {
                        button2.Background = Brushes.Red;
                    }
                    else
                    {
                        button2.Background = Brushes.White;
                    }
                }
            });
        }

        // Method to initialize the WCF service proxy
        private void InitializeService() {    
            try
            {
                string baseAddress = "net.tcp://localhost:8000/StockPriceService";
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
                var channel = new ChannelFactory<IStockPriceService>(binding);
                var endpoint = new EndpointAddress(baseAddress);
                proxy = channel.CreateChannel(endpoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Method to asynchronously get the latest stock price from the service
        private async void getLatestPrice(string stockName)
        {
            try
            {
                DateTime now = DateTime.Now;
                string formattedDateTime = now.ToString("d MMM yyyy HH:mm:ss");
                timing[stockName].Add(formattedDateTime);
                var response = await Task.Run(() => proxy?.getLatestPrice(stockName));
                stockPriceHistory[stockName].Add((double)response);
                Console.WriteLine(response.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

