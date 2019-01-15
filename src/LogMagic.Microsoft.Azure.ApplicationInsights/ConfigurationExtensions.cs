﻿using LogMagic.Microsoft.Azure.ApplicationInsights;
using LogMagic.Microsoft.Azure.ApplicationInsights.Writers;

namespace LogMagic
{
   /// <summary>
   /// Helpers methods to configure logging library
   /// </summary>
   public static class ConfigurationExtensions
   {
      /// <summary>
      /// Adds Azure Application Insights writer
      /// </summary>
      /// <param name="configuration">Configuration reference</param>
      /// <param name="instrumentationKey">Instrumentation key</param>
      /// <param name="flushOnWrite">When true, flush will be forced on every write</param>
      /// <param name="quickPulse">When true, enables Application Insights Live Streaming aka Quick Pulse</param>
      public static ILogConfiguration AddAzureApplicationInsights(this ILogConfiguration configuration, string instrumentationKey,
         bool flushOnWrite = false,
         bool quickPulse = false)
      {
         var options = new WriterOptions
         {
            FlushOnWrite = flushOnWrite,
            EnableQuickPulse = quickPulse
         };

         return configuration.AddWriter(new ApplicationInsightsWriter(instrumentationKey, options));
      }

      /// <summary>
      /// Adds Azure Application Insights writer
      /// </summary>
      /// <param name="configuration">Configuration reference</param>
      /// <param name="instrumentationKey">Instrumentation key</param>
      public static ILogConfiguration AddAzureApplicationInsights(this ILogConfiguration configuration, string instrumentationKey,
         WriterOptions options)
      {
         if (options == null) options = new WriterOptions();

         return configuration.AddWriter(new ApplicationInsightsWriter(instrumentationKey, options));
      }
   }
}
