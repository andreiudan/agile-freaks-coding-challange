using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace AgileFreaksCodingChallange
{
    public class CoffeeShop
    {
        public string Name { get; set; }
        public double CoordY { get; set; }
        public double CoordX { get; set; }
    }

    internal class Program
    {
        private static readonly string basePath = "https://raw.githubusercontent.com/Agilefreaks/test_oop/master/";
        
        private static string userInputURL = "";
        private static double userInputCoordX = 0D;
        private static double userInputCoordY = 0D;
        
        private static List<CoffeeShop> coffeeShops = new List<CoffeeShop>();
        private static KeyValuePair<string, double>[] closestCoffeeShops = new KeyValuePair<string, double>[3];

        static async Task Main(string[] args)
        {
            Initialize();
            bool userInputGathered = ReadUserInput();

            if (!userInputGathered)
            {
                return;
            }

            coffeeShops = await ReadFromCSV(basePath + userInputURL);

            //foreach (var pair in coffeeShops)
            //{
            //    Console.WriteLine(pair.Name + "  " + pair.CoordX.ToString() + "  " + pair.CoordY.ToString());
            //}

            FindClosestCoffeeShops();
            foreach (var pair in closestCoffeeShops)
            {
                Console.WriteLine(pair.Key + "  " + pair.Value.ToString());
            }
        }

        private static void Initialize()
        {
            for(int i = 0; i < closestCoffeeShops.Length; i++)
            {
                closestCoffeeShops[i] = KeyValuePair.Create(string.Empty, double.MaxValue);
            }
        }

        public static void FindClosestCoffeeShops()
        {
            foreach(CoffeeShop coffeeShop in coffeeShops)
            {
                double distance = Math.Sqrt(Math.Pow((coffeeShop.CoordX - userInputCoordY), 2) + Math.Pow((coffeeShop.CoordY - userInputCoordX), 2));

                bool newCoffeeShopAdded = false;
                KeyValuePair<string, double> changedCoffeeShop = KeyValuePair.Create("", 0D);

                for (int i = 0; i < closestCoffeeShops.Length; i++)
                {    
                    if(distance < closestCoffeeShops[i].Value && !newCoffeeShopAdded)
                    {
                        changedCoffeeShop = closestCoffeeShops[i];
                        closestCoffeeShops[i] = KeyValuePair.Create(coffeeShop.Name, distance);
                        newCoffeeShopAdded = true;
                    }
                    else if (newCoffeeShopAdded)
                    {
                        KeyValuePair<string, double> aux = KeyValuePair.Create("", 0D);
                        aux = closestCoffeeShops[i];
                        closestCoffeeShops[i] = changedCoffeeShop;
                        changedCoffeeShop = aux;
                    }
                }
            }
        }

        public static bool ReadUserInput()
        {
            Console.WriteLine("Insert the coordinate X, the coordinate Y and the shop data url separated by spaces.");
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

                    userInputCoordX = Convert.ToDouble(userInputs[0]);
                    userInputCoordY = Convert.ToDouble(userInputs[1]);
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
