using System;
using System.Collections.Generic;
using System.Text;

namespace LogMagic.Levels
{
    public static class LogLevelChecks
    {
      public static LogSeverity LogLevel { get; set; }
      public static bool Check(LogSeverity intended)
      {
         return (int) intended >= (int) LogLevel;
      }
    }
}
