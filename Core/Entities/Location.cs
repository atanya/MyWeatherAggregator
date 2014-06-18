﻿namespace WeatherAggregator.Core.Entities
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
    }
}
