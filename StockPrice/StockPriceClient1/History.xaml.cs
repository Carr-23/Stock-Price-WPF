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
using System.Windows.Shapes;

namespace StockPriceClient1
{
    // Define a class Item to represent individual stock data items
    public class Item
    {
        private string time;
        private double price;

        // Constructor to initialize time and price
        public Item(string time, double price)
        {
            Time = time;
            Price = price;
        }

        // Properties to access time and price
        public string Time { get; set; }
        public double Price { get; set; }
    }

    // Window class for displaying stock price history
    public partial class History : Window
    {
        // Default constructor
        public History()
        {
            InitializeComponent();
        }

        // Constructor to initialize with price history and timings
        public History(List<double> priceHistory, List<string> timings)
        {
            InitializeComponent();

            // Add each data point to the data grid
            for (int i = 0; i < Math.Min(priceHistory.Count, timings.Count); i++)
            {
                // Create a new Item object and add it to the data grid
                stockDataGrid.Items.Add(new Item(timings[i], priceHistory[i]));

                // Refresh the data grid to reflect changes
                stockDataGrid.Items.Refresh();
            }
        }
    }
}
