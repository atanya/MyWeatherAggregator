using System;
using System.Linq;
using Newtonsoft.Json;
using NLog;
using NLog.Common;

namespace WeatherAggregator.Core
{
    static class Logger
    {
        private const string LoggerName = "MovieServiceLogger";

        private static readonly NLog.Logger _nLog = LogManager.GetLogger(LoggerName);

        static Logger()
        {
            // enable internal logging to the console
            InternalLogger.LogToConsole = true;

            // enable internal logging to a file
            InternalLogger.LogFile = "c:\\log.txt";

            // set internal log level
            InternalLogger.LogLevel = LogLevel.Trace;
        }

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
