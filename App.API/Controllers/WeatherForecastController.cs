using App.API.Models;
using App.APIServices.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : DefaultController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, APIStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor)
        {
            _logger = logger;
        }

        //[HttpGet(Name = "GetWeatherForecast")]
        [Authorize]
        [Route("GetWeatherForecast"), HttpPost]
        [ServiceFilter(typeof(UniqueKeyMatching))]
        [ServiceFilter(typeof(KeyGenrate))]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
