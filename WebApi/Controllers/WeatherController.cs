using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Http;
using WeatherAggregator.Core;
using System.Web.Http.Results;
using System.Web.UI.WebControls;
using WeatherAggregator.Core;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Core.Interfaces;
using WeatherAggregator.Sources.Aspects;
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