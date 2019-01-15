using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace LogMagic.Microsoft.AspNetCore
{
   class LogMagicLoggerProvider : ILoggerProvider
   {
      private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

      public ILogger CreateLogger(string categoryName)
      {
         return _loggers.GetOrAdd(categoryName, CreateNewLogger);
      }

      private ILogger CreateNewLogger(string categoryName)
      {
         return new LogMagicLogger(categoryName);
      }


      public void Dispose()
      {

      }
   }
}
