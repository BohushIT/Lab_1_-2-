using Newtonsoft.Json.Linq; 
using StackExchange.Redis; 
using System.Net.Http; 

using System.Windows; 

using System.Diagnostics; 

namespace WpfApp1 
{

    public partial class MainWindow : Window 
    {
        // Статичне з'єднання з Redis-сервером
        private static ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect("localhost:6379");
        // Отримання бази даних з Redis
        private static IDatabase redisDb = redisConnection.GetDatabase();

        public MainWindow() 
        {
            InitializeComponent(); 
        }

        
        private async void GetWeatherWithoutRedis_Click(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text; 
            if (!string.IsNullOrWhiteSpace(city))
            {
                Stopwatch stopwatch = new Stopwatch(); 
                stopwatch.Start(); // Запуск таймера

                // для отримання даних погоди з API
                WeatherInfo weather = await GetWeatherFromApi(city);

                stopwatch.Stop();

                if (weather != null) 
                {
                    // Відображення отриманих даних
                    DisplayWeatherInfo(weather, "Without Redis (From API)");
                    WeatherResultTextBlock.Text += $"\nTime taken: {stopwatch.ElapsedMilliseconds} ms"; 

                    // Зберігаємо дані про погоду в Redis
                    string weatherJson = Newtonsoft.Json.JsonConvert.SerializeObject(weather); 
                    string cacheKey = $"weather_{city}"; // Створення ключа для кешу
                    redisDb.StringSet(cacheKey, weatherJson, TimeSpan.FromMinutes(3)); // Зберігання даних у Redis з терміном дії 3 хвилини
                }
                else 
                {
                    WeatherResultTextBlock.Text = "Error: Could not retrieve data."; // Відображення повідомлення про помилку
                }
            }
        }

        
        private async Task StoreRecentCity(string city)
        {
            const string recentCitiesKey = "recent_weather_cities"; // Ключ для списку нещодавно запитуваних міст

            // Додавання назви міста в кінець списку
            redisDb.ListRightPush(recentCitiesKey, city);

            // Якщо в списку більше 3 міст, видаляємо найстаріше
            if (redisDb.ListLength(recentCitiesKey) > 3)
            {
                redisDb.ListLeftPop(recentCitiesKey); // Видалення найстарішого елемента
            }
        }

       
        private async void GetWeatherWithRedis_Click(object sender, RoutedEventArgs e)
        {
            string city = CityTextBox.Text; 
            if (!string.IsNullOrWhiteSpace(city)) // Перевірка, чи не є рядок порожнім
            {
                string cacheKey = $"weather_{city}"; // Створення ключа для кешу
                string cachedWeather = redisDb.StringGet(cacheKey); // Спроба отримати дані з кешу

                Stopwatch stopwatch = new Stopwatch(); // Ініціалізація таймера
                stopwatch.Start(); // Запуск таймера

                if (string.IsNullOrEmpty(cachedWeather)) 
                {
                    WeatherResultTextBlock.Text = $"No weather data found for the city: {city} in cache.";
                    stopwatch.Stop(); 
                    return; 
                }
                else 
                {
                    try
                    {
                        
                        WeatherInfo weather = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfo>(cachedWeather);
                        stopwatch.Stop(); 
                        DisplayWeatherInfo(weather, "With Redis (From Cache)"); 
                        WeatherResultTextBlock.Text += $"\nTime taken: {stopwatch.ElapsedMilliseconds} ms"; 
                    }
                    catch (Exception ex) 
                    {
                        WeatherResultTextBlock.Text = $"Error deserializing data: {ex.Message}";
                    }
                }
            }
        }

        
        private async Task<WeatherInfo> GetWeatherFromApi(string city)
        {
            using (HttpClient client = new HttpClient()) //  HttpClient для виконання HTTP запитів
            {
                string apiKey = "23aa7477175d6c03dd1e2550b84460ce"; 
                // Формування URL запиту
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
                HttpResponseMessage response = await client.GetAsync(url); // Асинхронний запит до API

                if (response.IsSuccessStatusCode) 
                {
                    string weatherData = await response.Content.ReadAsStringAsync(); // Читання даних відповіді
                    JObject json = JObject.Parse(weatherData); // Парсинг JSON відповіді

                    // Створення об'єкта WeatherInfo з отриманими даними
                    WeatherInfo weatherInfo = new WeatherInfo
                    {
                        City = city,
                        Temperature = (double)json["main"]["temp"], // Отримання температури
                        Description = json["weather"][0]["description"].ToString(), // Отримання опису погоди
                        Humidity = (double)json["main"]["humidity"], // Отримання вологості
                        WindSpeed = (double)json["wind"]["speed"] // Отримання швидкості вітру
                    };

                    return weatherInfo; 
                }
                else 
                {
                    return null; 
                }
            }
        }

       
        private void DisplayWeatherInfo(WeatherInfo weather, string source)
        {
            
            WeatherResultTextBlock.Text = $"Weather information from {source}:\n" +
                                          $"City: {weather.City}\n" +
                                          $"Temperature: {weather.Temperature}°C\n" +
                                          $"Description: {weather.Description}\n" +
                                          $"Humidity: {weather.Humidity}%\n" +
                                          $"Wind Speed: {weather.WindSpeed} m/s";
        }
    }
}
