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
        private static readonly string basePath = "https://raw.githubusercontent.com/Agilefreaks/test_oop/master/";
        private static string userInputURL = "";
        private static decimal userInputCoordX = 0M;
        private static decimal userInputCoordY = 0M;

        static async Task Main(string[] args)
        {
            bool userInputGathered = ReadUserInput();

            if (!userInputGathered)
            {
                return;
            }

            List<CoffeeShop> coffeShops = new List<CoffeeShop>();
            coffeShops = await ReadFromCSV(basePath + userInputURL);
        }

        public static bool ReadUserInput()
        {
            Console.WriteLine("Insert the coordinate x, the coordinate Y and the shop data url separated by spaces.");
            Console.WriteLine("Insert '0' if you don't want to insert any data.");

            while (true) 
            {
                try
                {
                    string userInput = Console.ReadLine() ?? throw new ArgumentNullException("Empty input is not allowed!");

                    if(userInput == string.Empty)
                    {
                        throw new ArgumentException("Input cannot be empty!");
                    }

                    if (userInput.Equals("0"))
                    {
                        return false;
                    }

                    string[] userInputs = userInput.Split(' ');

                    if (userInputs.Length != 3) 
                    {
                        throw new ArgumentException("Three arguments are expected!");
                    }

                    userInputCoordX = Convert.ToDecimal(userInputs[0]);
                    userInputCoordY = Convert.ToDecimal(userInputs[1]);
                    userInputURL = userInputs[2];

                    if (!userInputURL.Contains(".csv"))
                    {
                        throw new ArgumentException("The file must be of type csv!");
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
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
        
            IEnumerable<CoffeeShop> records = csvReader.GetRecords<CoffeeShop>();

            foreach (CoffeeShop record in records) 
            {
                list.Add(record);
            }

            return list;
        }
    }
}
