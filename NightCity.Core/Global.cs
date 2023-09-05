using NLog;
using System;

namespace NightCity.Core
{
    public class Global
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        public static void Log(string content, bool isError = false)
        {
            Console.WriteLine(content);
            if (isError)
                log.Error(content);
            else
                log.Info(content);
        }
    }
}
