using System;
using System.Collections.Generic;
using WeatherAggregator.Core.Aspects;

namespace WeatherAggregator.Core.Entities
{
    public class Weather
    {
        [DateOnly]
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public int Cloudness { get; set; }
        public Precipitation Precipitation { get; set; }

        public List<SourcedWeather> Sources;
    }
}
