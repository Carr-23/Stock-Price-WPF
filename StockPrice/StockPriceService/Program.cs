using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockPriceService
{
    internal class Program
    {
        // Define the service contract for the Stock Price Service
        [ServiceContract]
        public interface IStockPriceService
        {
            // Operation contract to get the latest price of a stock
            [OperationContract]
            double getLatestPrice(string stockName);
        }

        // Implementation of the Stock Price Service
        public class StockPriceService : IStockPriceService
        {
            private Dictionary<string, List<double>> stockPriceHistory = new Dictionary<string, List<double>>();
            private Random random = new Random();

            // Constructor to initialize the stock price history and start price generation
            public StockPriceService()
            {
                // Setting default entries
                stockPriceHistory["Stock1"] = new List<double>();
                stockPriceHistory["Stock2"] = new List<double>();

                PriceGeneration();
            }

            // Asynchronous method to generate random stock prices every second
            private void PriceGeneration()
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        // Simulate price generation for Stock 1 and Stock 2
                        GenerateStockPrice("Stock1");
                        GenerateStockPrice("Stock2");

                        await Task.Delay(1000);
                    }
                });
            }

            // Generate a random stock price based on the stock name
            private void GenerateStockPrice(string stockName)
            {
                double newPrice = 0;
                if (stockName == "Stock1")
                {
                    newPrice = 240 + (270 - 240) * random.NextDouble();
                }
                else
                {
                    newPrice = 180 + (210 - 180) * random.NextDouble();
                }

                stockPriceHistory[stockName].Add(Math.Round(newPrice, 2)); // Add the new price to the history
            }

            // Method to get the latest price of a stock
            public double getLatestPrice(string stockName)
            {
                return stockPriceHistory[stockName].Last();
            }

            // Entry point of the program
            static void Main(string[] args)
            {
                string baseAddress = "net.tcp://localhost:8000/StockPriceService"; 
                ServiceHost host = new ServiceHost(typeof(StockPriceService), new Uri(baseAddress));
                var binding = new NetTcpBinding(SecurityMode.None);
                // Add the service endpoint
                host.AddServiceEndpoint(typeof(IStockPriceService), binding, "");
                // Open the service host to start listening for client requests
                host.Open();
                // Wait for user input before exiting (press Enter to close the host)
                Console.ReadLine();
                host.Close();
            }

        }
    }
}
