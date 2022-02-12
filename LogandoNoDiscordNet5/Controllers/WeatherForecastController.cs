using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogandoNoDiscordNet5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LogandoNoDiscordNet5.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecastModel> Get()
        {
            _logger.LogInformation("Olá! Você recebeu uma mensagem pelo Discord.");

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastModel
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public IEnumerable<WeatherForecastModel> Post()
        {
            var i = 0;

            var x = 5 / i; // <-- Dispara a exception em runtime

            return new List<WeatherForecastModel>();
        }
    }
}
