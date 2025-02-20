using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace AgileFreaksCodingChallange
{
    public class CoffeeShop
    {
        public string Name { get; set; }
        public decimal CoordX { get; set; }
        public decimal CoordY { get; set; }
    }

    internal class Program
    {
        private static readonly string fileURL = "https://raw.githubusercontent.com/Agilefreaks/test_oop/master/coffee_shops.csv";

        static async Task Main(string[] args)
        {
            List<CoffeeShop> coffeShops = new List<CoffeeShop>();
            coffeShops = await ReadFromCSV(fileURL);
        }

        public static async Task<List<CoffeeShop>> ReadFromCSV(string URL)
        {
            List<CoffeeShop> list = new List<CoffeeShop>();

            IReaderConfiguration csvReaderConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Delimiter = ",",
            };

            using HttpClient httpClient = new HttpClient();
            using Stream stream = await httpClient.GetStreamAsync(URL);
            using StreamReader streamReader = new StreamReader(stream);
            using CsvReader csvReader = new CsvReader(streamReader, csvReaderConfiguration);
        
            var records = csvReader.GetRecords<CoffeeShop>();

            foreach (var record in records) 
            {
                list.Add(record);
            }

            return list;
        }
    }
}
