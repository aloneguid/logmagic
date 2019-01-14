using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LogMagic.PerfCounters.Windows;

namespace LogMagic
{
   /// <summary>
   /// Performance counters configuration extensions
   /// </summary>
   public static class PerfConfigurationExtensions
   {
      /// <summary>
      /// Add default platform performance counters. In Windows on desktop .NET or .NET Standard 2.0 adds a few
      /// default performance counters, otherwise does nothing.
      /// </summary>
      public static ILogConfiguration PlatformDefault(this IPerformanceCounterConfiguration configuration)
      {

         string processName = Process.GetCurrentProcess().ProcessName;

         configuration.WindowsCounter("Machine CPU Load (%)", "Processor", "% Processor Time", "_Total");
         configuration.WindowsCounter("Machine Available Memory (bytes)", "Memory", "Available Bytes", null);
         configuration.WindowsCounter("Process CPU Load (%)", "Process", "% Processor Time", processName);
         configuration.WindowsCounter("Process Private Memory (bytes)", "Process", "Private Bytes", processName);
         configuration.WindowsCounter("Process IO Data (bytes/sec)", "Process", "IO Data Bytes/sec", processName);

         return configuration.Custom(null);
      }

      /// <summary>
      /// Windows based performance counter. This w
      /// </summary>
      public static ILogConfiguration WindowsCounter(this IPerformanceCounterConfiguration configuration,
         string name, string categoryName, string counterName, string instanceName)
      {
         return configuration.Custom(new WindowsPerformanceCounter(name, categoryName, counterName, instanceName));
      }
   }
}
