using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LogMagic.Tokenisation;

namespace LogMagic.Writers
{
   /// <summary>
   /// Receives log chunks into standard <see cref="Trace"/>
   /// </summary>
   class TraceLogWriter : ILogWriter
   {
      private readonly FormattedString _format;

      /// <summary>
      /// Creates an instance with standard formatter
      /// </summary>
      public TraceLogWriter(string format)
      {
         _format = format == null ? null : FormattedString.Parse(format, null);
      }

      /// <summary>
      /// Nothing to dispose
      /// </summary>
      public void Dispose()
      {
      }

      public void Write(IEnumerable<LogEvent> events)
      {
         foreach(LogEvent e in events)
         {
            string line = TextFormatter.Format(e, _format);

            if(e.ErrorException != null)
            {
               Trace.TraceError(line);
            }
            else if(e.Severity == LogSeverity.Warning)
            {
               Trace.TraceWarning(line);
            }
            else
            {
               Trace.TraceInformation(line);
            }
         }
      }
   }
}