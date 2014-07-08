using System;
using NUnit.Framework;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Sources.WorldWeatherOnline;

namespace Tests
{
    [TestFixture]
    public class WorldWeatherOnlineTests
    {
        [Test]
        public void TheTest()
        {
            var source = new WorldWeatherOnlineSource();
            var result = source.GetWeather(new DateRange {From = DateTime.Now, To = DateTime.Now.AddDays(2)},
                new Location {Latitude = 44.616649, Longitude = 33.52536});
            Assert.IsNotEmpty(result);
        }
    }
}
