using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using WeatherAggegator.Helpers;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Core.Interfaces;
using WeatherAggregator.Sources.OpenWeatherMap.Api;

namespace WeatherAggregator.Sources.OpenWeatherMap
{
    /// <summary>
    /// http://openweathermap.org
    /// </summary>
    public class OpenWeatherMapSource : ISource
    {
        private const string ApiUri = "http://api.openweathermap.org/data/2.5/forecast/daily?lat={0}&lon={1}&cnt=15&mode=json";

        public Guid Id
        {
            get
            {
                return new Guid("b897eb71-990a-4155-bbf8-682094bfcba0");
            }
        }

        public string Name
        {
            get
            {
                return "Open Weather Map";
            }
        }

        public string Snippet
        {
            get
            {
                return "<a href='http://openweathermap.org'>OpenWeatherMap</a>";
            }
        }

        public int ForecastMaxDays
        {
            get
            {
                return 16;
            }
        }

        public List<Weather> GetWeather(DateRange dateRange, Location location)
        {
            var url = string.Format(ApiUri, location.Latitude, location.Longitude);

            var response = WebRequest.Create(url).GetResponse();
            var json = JsonConvert.DeserializeObject<WeatherResponse>(new StreamReader(response.GetResponseStream()).ReadToEnd());
            var result = new List<Weather>();

            foreach (var item in json.list)
            {
                var date = DateTimeHelpers.FromTimeStamp(item.dt);
                if (dateRange.Contains(date.Date))
                {
                    var weather = new Weather
                    {
                        Cloudness = item.clouds,
                        Date = date.Date,
                        Temperature = (item.temp.max + item.temp.min) / 2 - 273.15, //Take average and convert from Kelvin to Celsius
                        Precipitation = GetPrecipitation(item.weather.Select(w => w.id))
                    };

                    result.Add(weather);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts weather code to <see cref="Precipitation"/> according to
        /// http://openweathermap.org/weather-conditions
        /// </summary>
        private Precipitation GetPrecipitation(IEnumerable<int> weaterCode)
        {
            var result = Precipitation.None;

            if (weaterCode.Any(w => w== 906)) result = Precipitation.Hail;
            else if (weaterCode.Any(w => w>= 600 && w < 700)) result = Precipitation.Snow;
            else if (weaterCode.Any(w => w >= 200 && w < 600)) result = Precipitation.Rain;

            return result;
        }
    }
}
