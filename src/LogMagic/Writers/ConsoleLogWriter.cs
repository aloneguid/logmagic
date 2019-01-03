using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogMagic.Tokenisation;

namespace LogMagic.Writers
{
   /// <summary>
   /// Outputs messages to system console and ideal for server logging. It doesn't do anything fancy 
   /// unlike <see cref="PoshConsoleLogWriter"/>
   /// </summary>
   class ConsoleLogWriter : ILogWriter
   {
      private readonly FormattedString _format;

      /// <summary>
      /// Creates class instance
      /// </summary>
      public ConsoleLogWriter(string format)
      {
         _format = format == null ? null : FormattedString.Parse(format, null);
      }

      /// <summary>
      /// There is nothing to dispose in the console
      /// </summary>
      public void Dispose()
      {
      }

      public void Write(IEnumerable<LogEvent> events)
      {
         foreach(LogEvent e in events)
         {
            Console.WriteLine(TextFormatter.Format(e, _format));
         }
      }
   }
}
