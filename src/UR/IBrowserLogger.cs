using Microsoft.Extensions.Logging;

namespace UR;

public interface IBrowserLogger
{
    void Log(LogLevel logLevel, string logMessage);
}
