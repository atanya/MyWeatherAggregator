﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WeatherAggregator.Core;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.WebApi.Models;

namespace WeatherAggregator.WebApi.Controllers
{
    public class WeatherController : ApiController
    {
        private readonly CoreFacade _facade = new CoreFacade();

        [HttpGet]
        public List<SourceModel> Sources()
        {
            return _facade.GetSources().Select(SourceModel.Map).ToList();
        }

        [HttpPost]
        public List<Weather> GetWeather(WeatherRequest request)
        {
            var result = _facade.GetWeather(request.Sources, request.DateRange, request.Location);
            return result;
        }

        ~WeatherController()
        {
            _facade.Dispose();
        }
    }
}