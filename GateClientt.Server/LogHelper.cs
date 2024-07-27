using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateClientt.Server
{
    public static class LogHelper
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();
        static LogHelper()
        {
            var config = new LoggingConfiguration();
            // info日志
            var logTarget = new FileTarget
            {
                FileName = "${basedir}/log/${shortdate}.log",
                Layout = @"${date:format=HH\:mm\:ss.fff} ${message} ${exception:format=tostring}"
            };
            config.AddTarget("log", logTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, logTarget));

            LogManager.Configuration = config;

        }


        public static void Log(params string[] args) => logger.Info(string.Join(" ", args));

    }
}
