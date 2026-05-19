using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;
     
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory http, IConfiguration config)
        {
            _logger = logger;
            _http = http;
            _config = config;
        }
        public IActionResult Index()
        {
            return View(new WeatherModel());
        }
        [HttpPost]
        public async Task<IActionResult> Index(string city)
        {
            var apiKey = _config["WeatherApiKey"];
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

            var client = _http.CreateClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return View(new WeatherModel { Error = "City not found!" });
            }

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            double tempC = doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble();
            double tempF = (tempC * 9 / 5) + 32;
            string cityName = doc.RootElement.GetProperty("name").GetString();

            return View(new WeatherModel
            {
                City = cityName,
                TempC = Math.Round(tempC, 1),
                TempF = Math.Round(tempF, 1)
            });
        }
       

        public IActionResult Privacy()
        {
            return View();
        }
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
