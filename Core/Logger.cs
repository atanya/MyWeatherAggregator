using System;
using System.Linq;
using log4net;
using Newtonsoft.Json;

namespace WeatherAggregator.Core
{
    static class Logger
    {
        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure();
            _log4net = LogManager.GetLogger(typeof(object));
        }

        private static readonly ILog _log4net;

        public static void Info(string format, params object[] @params)
        {
            var jsonParams = @params.Select(p => JsonConvert.SerializeObject(p));
            _log4net.InfoFormat(format, (object[]) jsonParams.ToArray());
        }

        public static void Error(Exception e)
        {
            _log4net.Error("Unhandled exception", e);
        }
    }
}
