using System;
using System.Collections.Generic;
using System.Text;
using LogMagic.Enrichers;

namespace LogMagic.Filters
{
   class MinLogSeverityFilter : IFilter
   {
      private readonly LogSeverity _minSeverity;

      public MinLogSeverityFilter(LogSeverity minSeverity)
      {
         _minSeverity = minSeverity;
      }

      public bool Match(LogEvent e)
      {
         if (e == null) return true;

         LogSeverity severity = e.GetProperty<LogSeverity>(KnownProperty.Severity, LogSeverity.Verbose);

         return severity >= _minSeverity;
      }
   }
}
