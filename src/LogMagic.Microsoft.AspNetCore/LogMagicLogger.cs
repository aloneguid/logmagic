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

         var statePairs = state as IEnumerable<KeyValuePair<string, object>>;
         if (statePairs == null) return;

         var stateDictionary = new Dictionary<string, object>();
         foreach(KeyValuePair<string, object> pair in statePairs)
         {
            stateDictionary[pair.Key] = pair.Value;
         }

         string message = formatter(state, exception);

         _log.Write(ToLogSeverity(logLevel), message, statePairs);
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
