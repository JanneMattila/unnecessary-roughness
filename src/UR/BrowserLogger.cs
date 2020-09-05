using System;
using Microsoft.Extensions.Logging;

namespace UR
{
    public class BrowserLogger : IBrowserLogger
    {
        public void Log(LogLevel logLevel, string logMessage)
        {
            Console.WriteLine($"{logLevel}: {logMessage}");
        }
    }
}
