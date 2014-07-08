using System;
using System.Collections.Generic;
using System.Configuration;
using ForecastIO;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Core.Interfaces;

namespace WeatherAggregator.Sources
{
    public class ForecastioSource:ISource
    {
        public Guid Id { get { return new Guid("2D498C9F-8294-4A47-97E4-6CF1B1800F3E"); } }
        public string Name { get { return "Forecast.io";  } }
        public string Snippet { get { return "<a href='http://forecast.io/'>Powered by Forecast</a>"; } }
        public int ForecastMaxDays { get { return 0; } }

        private string ApiKey
        {
            get { return ConfigurationManager.AppSettings["ForecastIOKey"]; }
        }
        public List<Weather> GetWeather(DateRange dateRange, Location location)
        {
            var result = new List<Weather>();
            var day = dateRange.From;
            while (day <= dateRange.To)
            {
                var request = new ForecastIORequest(ApiKey, (float)location.Latitude, (float)location.Longitude, day.Date, Unit.si);
                var response = request.Get();
                if (response.daily.data.Count > 0)
                {
                    var dailyForecast = response.daily.data[0];
                    var temp = (dailyForecast.apparentTemperatureMax + dailyForecast.apparentTemperatureMin)/2;
                    var cloudCover = (int)(dailyForecast.cloudCover * 100);
                    var precip = Precipitation.None;
                    if (dailyForecast.precipIntensity > 50)
                    {
                        precip = temp > 0 ? Precipitation.Rain : Precipitation.Snow;
                    }
                    var w = new Weather { Date = day.Date, Temperature = temp, Cloudness = cloudCover, Precipitation = precip};
                    result.Add(w);
                }
                day = day.AddDays(1);
            }

            return result;
        }
    }
}
