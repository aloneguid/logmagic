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
         if (state == null) return null;
         if (!(state is IDictionary<string, object> stateDic)) return null;

         return _log.Context(stateDic);
      }

      public bool IsEnabled(LogLevel logLevel)
      {
         //all levels are enabled by default as filtering happens in LogMagic
         return true;
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
         string message = formatter(state, exception);

         var stateValues = state as IEnumerable<KeyValuePair<string, object>>;
         if (stateValues == null) return;

         _log.Write(message, ToLogSeverity(logLevel), stateValues);
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
