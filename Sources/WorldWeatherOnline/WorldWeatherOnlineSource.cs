using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using Newtonsoft.Json;
using WeatherAggegator.Helpers;
using WeatherAggregator.Core.Aspects;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Core.Interfaces;
using WeatherAggregator.Sources.WorldWeatherOnline.Api;
using Weather = WeatherAggregator.Core.Entities.Weather;

namespace WeatherAggregator.Sources.WorldWeatherOnline
{
    public class WorldWeatherOnlineSource : ISource
    {
        private const string ApiUri = "http://api.worldweatheronline.com/free/v1/weather.ashx?" +
            "q={0},{1}&format=json&num_of_days=5&key={2}";

        private string ApiKey
        {
            get { return ConfigurationManager.AppSettings["WorldWeatherOnlineKey"]; }
        }

        public Guid Id
        {
            get
            {
                return new Guid("a7720ab2-bb98-4a48-a361-7a084c91305f");
            }
        }

        public string Name
        {
            get
            {
                return "World Weather Online";
            }
        }

        public string Snippet
        {
            get
            {
                return "<a href='http://www.worldweatheronline.com/'>World Weather Online</a>";
            }
        }

        public int ForecastMaxDays
        {
            get
            {
                return 5;
            }
        }

        public List<Weather> GetWeather(DateRange dateRange, Location location)
        {
            var json = GetWeatherFromService(location);

            var weatherCodeMapping = Assembly.GetExecutingAssembly()
                .GetResource("WeatherAggregator.Sources.WorldWeatherOnline.WeatherCodes.xml");
            var xMapping = XElement.Parse(weatherCodeMapping);

            if (json == null || json.data == null || json.data.weather == null) return new List<Weather>();

            var result = json.data.weather.Where(w => dateRange.Contains(w.date)).Select(w => new Weather
            {
                Date = w.date,
                Temperature = (w.tempMinC + w.tempMaxC) / 2,
                Precipitation = GetPrecipitationByCode(w.weatherCode, xMapping),
                Cloudness = GetCloudnessByCode(w.weatherCode, xMapping)
            }).ToList();

            return result;
        }

        [MethodLog]
        private Response GetWeatherFromService(Location location)
        {
            var url = string.Format(ApiUri, location.Latitude, location.Longitude, ApiKey);

            var response = WebRequest.Create(url).GetResponse();
            var json = JsonConvert.DeserializeObject<Response>(new StreamReader(response.GetResponseStream()).ReadToEnd());
            return json;
        }

        private Precipitation GetPrecipitationByCode(string code, XElement xMapping)
        {
            var node = xMapping.Nodes().OfType<XElement>().Single(n => n.Descendants("code").Single().Value == code).Descendants("precipitation").Single();
            return (Precipitation) Enum.Parse(typeof (Precipitation), node.Value);
        }

        private int GetCloudnessByCode(string code, XElement xMapping)
        {
            var node = xMapping.Nodes().OfType<XElement>().Single(n => n.Descendants("code").Single().Value == code).Descendants("Cloudness").Single();
            return int.Parse(node.Value);
        }
    }
}
