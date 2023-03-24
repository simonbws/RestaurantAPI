﻿using Microsoft.AspNetCore.Mvc;

namespace RestaurantAPI
{
    public class WeatherForecastService
    {
        public class WeatherForecastController : IWeatherForecastService
        {
            private static readonly string[] Summaries = new[]
            {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
            public IEnumerable<WeatherForecast> Get(int count, int minTemperature, int maxTemperature)
            {
                return Enumerable.Range(1, count).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(minTemperature, maxTemperature),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
            }
        }
    }
}
