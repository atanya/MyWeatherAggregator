using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WeatherAggregator.Core;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.WebApi.Models;

namespace WeatherAggregator.WebApi.Controllers
{
    public class WeatherController : BaseController
    {
        [HttpGet]
        public List<SourceModel> Sources()
        {
            return Facade.GetSources().Select(SourceModel.Map).ToList();
        }

        [HttpPost]
        public List<Weather> GetWeather(WeatherRequest request)
        {
            var result = Facade.GetWeather(request.Sources, request.DateRange, request.Location);
            return result;
        }
    }
}