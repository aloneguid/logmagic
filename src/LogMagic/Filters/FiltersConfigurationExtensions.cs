using LogMagic.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogMagic
{
   /// <summary>
   /// Contains extensions for configuring logging filters
   /// </summary>
   public static class FiltersConfigurationExtensions
   {
      /// <summary>
      /// Applies a lambda function
      /// </summary>
      public static ILogConfiguration Lambda(this IFilterConfiguration configuration, Func<LogEvent, bool> func)
      {
         return configuration.Filter(new LambdaFilter(func));
      }

      /// <summary>
      /// Applies log severity filter by matching everything that is greater than or equal to <paramref name="minSeverity"/>
      /// </summary>
      public static ILogConfiguration MinLogSeverity(this IFilterConfiguration configuration, LogSeverity minSeverity)
      {
         return configuration.Filter(new MinLogSeverityFilter(minSeverity));
      }
   }
}
