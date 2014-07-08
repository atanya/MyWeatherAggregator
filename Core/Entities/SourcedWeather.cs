using System;

namespace WeatherAggregator.Core.Entities
{
    public class SourcedWeather: Weather
    {
        public string SourceName { get; set; }

        public SourcedWeather()
        {
        }

        public SourcedWeather(Weather weather, string sourceName)
        {
            Date = weather.Date;
            Temperature = Math.Round(weather.Temperature, 2);
            Cloudness = weather.Cloudness;
            Precipitation = weather.Precipitation;
            SourceName = sourceName;
        }
    }
}
