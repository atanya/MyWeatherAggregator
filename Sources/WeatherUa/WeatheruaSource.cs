using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Core.Interfaces;
using WeatherAggregator.Sources.WeatherUa;

namespace Sources
{
    public class WeatheruaSource: ISource
    {

        private const string SnippetHtml = "<p><a href='http://www.weather.ua/'><img src='http://www.weather.ua/images/logo-weather-ua.gif' alt='Weather.ua логотип' width='253' height='70'><br /></p>";

        public Guid Id { get { return new Guid("F1465F7E-DFAC-4D77-A717-0613B05A5281"); } }
        public string Name { get { return "Weather.ua"; } }
        public string Snippet { get { return SnippetHtml; } }
        public int ForecastMaxDays { get { return 5; } }

        public List<Weather> GetWeather(DateRange dateRange, Location location)
        {
            var result = new List<Weather>();

            var city = GetCity(location, result);
            if (city == null)
                return result;

            var xml = XDocument.Load(string.Format("http://xml.weather.co.ua/1.2/forecast/{0}?dayf=5&userid=wa.com", city.ID));
            var forecasts = xml.Descendants("day").ToList();
            var day = dateRange.From;
            while (day <= dateRange.To)
            {
                var actual = forecasts.Where(f => f.Attribute("date").Value == day.Date.ToString("yyyy-MM-dd")).ToList();
                if (actual.Any())
                {
                    var weather = new Weather
                    {
                        Date = day.Date,
                        Temperature = GetTemperature(actual),
                        Cloudness = GetCloudness(actual)
                    };
                    weather.Precipitation = GetPrecipitation(actual) < 50
                        ? Precipitation.None
                        : (weather.Temperature < 0 ? Precipitation.Snow : Precipitation.Rain);
                    result.Add(weather);
                }
                day = day.AddDays(1);
            }

            return result;
        }

        private static int GetPrecipitation(IEnumerable<XElement> actual)
        {
            var ppcp = actual.Descendants("ppcp").Select(n => Convert.ToInt32(n.Value));
            return Convert.ToInt32(ppcp.Sum() / ppcp.Count());
        }
        
        private static int GetCloudness(IEnumerable<XElement> actual)
        {
            var cloud = actual.Descendants("cloud").Select(n => Convert.ToInt32(n.Value));
            return Convert.ToInt32(cloud.Sum() / cloud.Count());
        }
        
        private static double GetTemperature(IEnumerable<XElement> actual)
        {
            //all mins for day
            var minT = actual.Descendants("t").Descendants("min").Select(n => Convert.ToDouble(n.Value));
            //all maxes for day
            var maxT = actual.Descendants("t").Descendants("max").Select(n => Convert.ToDouble(n.Value)).ToList();
            maxT.AddRange(minT);
            return maxT.Sum()/maxT.Count;
        }

        private static WeatheruaCity GetCity(Location location, List<Weather> result)
        {
            if (string.IsNullOrEmpty(location.City))
                return null;

            var cities = WeatheruaLoader.LoadCities();
            if (cities == null) 
                return null;

            var allCities = cities.Where(c => c.Name == location.City);
            var city = allCities.Count() > 1
                ? allCities.FirstOrDefault(c => c.Region == location.Region && c.Country == location.Country)
                : allCities.FirstOrDefault();

            return city;
        }
    }
}
