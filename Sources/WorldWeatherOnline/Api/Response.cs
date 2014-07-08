using System.Collections.Generic;

namespace WeatherAggregator.Sources.WorldWeatherOnline.Api
{
    class Response
    {
        public Meta meta;
        public List<Error> results;
        public Data data;
    }
}
