using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace LogMagic.Microsoft.AspNetCore
{
   class LogMagicLogger : ILogger
   {
      private readonly ILog _log;
      private readonly string _categoryName;

      public LogMagicLogger(string categoryName)
      {
         _log = L.G(categoryName);
         _categoryName = categoryName;
      }

      public IDisposable BeginScope<TState>(TState state)
      {
         //we don't need scope here because LogMagic has it's own API for scoping.
         return null;
      }

      public bool IsEnabled(LogLevel logLevel)
      {
         //all levels are enabled by default as filtering happens in LogMagic
         return true;
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
         string message = formatter(state, exception);

         _log.Write(message, new KeyValuePair<string, object>(KnownProperty.Severity, ToLogSeverity(logLevel)));
      }

      private LogSeverity ToLogSeverity(LogLevel logLevel)
      {
         switch (logLevel)
         {
            case LogLevel.Information:
               return LogSeverity.Information;
            case LogLevel.Warning:
               return LogSeverity.Warning;
            case LogLevel.Error:
               return LogSeverity.Error;
            case LogLevel.Critical:
               return LogSeverity.Critical;
            default:
               return LogSeverity.Verbose;
         }
      }
   }
}
