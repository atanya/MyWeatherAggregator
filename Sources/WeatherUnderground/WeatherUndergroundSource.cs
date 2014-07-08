using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web.Script.Serialization;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Core.Interfaces;

namespace WeatherAggregator.Sources.WeatherUnderground
{
    public class WeatherUndergroundSource : ISource
    {
  
        #region Public Properties

        private string snippetHtml = @"<p><a href='http://www.wunderground.com/'><img src='http://icons.wxug.com/i/o/logo-footer.png'></a></p>";

        public Guid Id { get { return new Guid("b0fce04e-4cd6-4ba9-8e75-4e8f1a51c33e"); } }
        public string Name { get { return "Weather Undergroud"; } }

        public string Snippet { get { return snippetHtml; } }
        public int ForecastMaxDays { get { return 4; } }

        #endregion


        #region Public Methods

        public List<Weather> GetWeather(DateRange dateRange, Location location)
        {
            // dateRange is not used because method returns forecast for today and three more days

            ThreeDaysForecastModel model = ThreeDaysForecast(location);
            List<Weather> result = ConvertForecastModel(model);

            return result;
        }

        #endregion

        private string ApiKey
        {
            get { return ConfigurationManager.AppSettings["WeatherUnderGroundIOKey"]; }
        }

        #region Private Methods

        private List<Weather> ConvertForecastModel(ThreeDaysForecastModel model)
        {
            List<Weather> result = new List<Weather>();
            if (model != null && model.Forecast != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    Weather resultItem = new Weather();

                    resultItem.Temperature =
                        DataExtractor.GetAverageTemperature(model.Forecast.SimpleForecast.ForecastDay[i]);
                    resultItem.Cloudness = DataExtractor.GetCloudness(model.Forecast.SimpleForecast.ForecastDay[i]);
                    resultItem.Precipitation =
                        DataExtractor.GetPrecipitation(model.Forecast.SimpleForecast.ForecastDay[i]);
                    var forecastDayDate = model.Forecast.SimpleForecast.ForecastDay[i].Date;
                    resultItem.Date = new DateTime(forecastDayDate.Year, forecastDayDate.Month, forecastDayDate.Day);
                    result.Add(resultItem);
                }
            }

            return result;
        }

        private ThreeDaysForecastModel ThreeDaysForecast(Location location)
        {
            using (var client = new WebClient())
            {
                ThreeDaysForecastModel model = null;
                try
                {

                    string requestUrl = string.Format(@"http://api.wunderground.com/api/{0}/forecast/q/{1},{2}.json",
                        ApiKey, location.Latitude, location.Longitude);
                    var json = client.DownloadString(requestUrl);
                    var serializer = new JavaScriptSerializer();

                    model = serializer.Deserialize<ThreeDaysForecastModel>(json);
                }
                catch (Exception e)
                {
                    
                }
                return model;
            }
        }


        #endregion
    }





}

