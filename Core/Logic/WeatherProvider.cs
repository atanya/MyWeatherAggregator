using System;
using System.Collections.Generic;
using System.Linq;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Core.Interfaces;

namespace WeatherAggregator.Core.Logic
{
    public class WeatherProvider
    {
        private readonly List<ISource> _sources;

        public WeatherProvider(List<ISource> sources)
        {
            _sources = sources;
        }
        
        public List<Weather> GetWeather(DateRange dateRange, Location location)
        {
            var result = new List<Weather>();
            var weathers = new List<SourcedWeather>();
            foreach (var source in _sources)
            {
                weathers.AddRange(source.GetWeather(dateRange, location).Select(s => new SourcedWeather(s, source.Name)).ToList());
            }
            var weather = weathers.GroupBy(r => r.Date).Select(g => new Weather
            {
                Temperature = (int)Math.Round(g.Average(r => r.Temperature)),
                Cloudness = (int)g.Average(r => r.Cloudness),
                Precipitation = (Precipitation)(int)g.Average(r => (int)r.Precipitation),
                Date = g.Key,
                Sources = g.ToList()
            }).ToList();
            result.AddRange(weather);

            return result;
        }
    }
}
