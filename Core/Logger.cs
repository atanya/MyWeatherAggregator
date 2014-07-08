using System;
using System.Linq;
using Newtonsoft.Json;
using NLog;
using NLog.Common;

namespace WeatherAggregator.Core
{
    static class Logger
    {
        private const string LoggerName = "WeatherLogger";

        private static readonly NLog.Logger _nLog = LogManager.GetLogger(LoggerName);

        public static void Info(string format, params object[] @params)
        {
            var jsonParams = @params.Select(p => JsonConvert.SerializeObject(p));
            _nLog.Info(format, (object[]) jsonParams.ToArray());
        }

        public static void Error(Exception e)
        {
            _nLog.Error("Unhandled exception", e);
        }
    }
}
