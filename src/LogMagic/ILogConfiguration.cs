using System;
using System.Collections.Generic;
using LogMagic.PerfCounters;

namespace LogMagic
{
   #region [ Simple interfaces for configuration entries ]

   /// <summary>
   /// Logging filter
   /// </summary>
   public interface IFilter
   {
      /// <summary>
      /// Return TRUE to match this log event
      /// </summary>
      /// <param name="e"></param>
      /// <returns></returns>
      bool Match(LogEvent e);
   }

   /// <summary>
   /// Configuration of logging filters
   /// </summary>
   public interface IFilterConfiguration
   {
      /// <summary>
      /// Adds a custom filter
      /// </summary>
      /// <param name="filter">Filter reference</param>
      /// <returns>Log configuration</returns>
      ILogConfiguration Filter(IFilter filter);
   }

   /// <summary>
   /// Configuration of performance counters
   /// </summary>
   public interface IPerformanceCounterConfiguration
   {
      /// <summary>
      /// Add custom performance counter
      /// </summary>
      ILogConfiguration Custom(IPerformanceCounter performanceCounter);

      /// <summary>
      /// Set custom sampling interval
      /// </summary>
      ILogConfiguration WithSamplingInterval(TimeSpan samplingInterval);
   }

   #endregion

   /// <summary>
   /// Entry point to logging configuration
   /// </summary>
   public interface ILogConfiguration
   {
      /// <summary>
      /// Resets configuration by clearing all writers etc.
      /// </summary>
      void Reset();

      /// <summary>
      /// Calls dispose on all configured writers
      /// </summary>
      void Shutdown();

      /// <summary>
      /// Get configured performance counters
      /// </summary>
      IReadOnlyCollection<IPerformanceCounter> PerformanceCounters { get; }

      /// <summary>
      /// Entry point to enrichers configuration
      /// </summary>
      IEnricherConfiguration EnrichWith { get; }

      /// <summary>
      /// Entry point to writers configuration
      /// </summary>
      IWriterConfiguration WriteTo { get; }

      /// <summary>
      /// Entry point to filters configuration
      /// </summary>
      IFilterConfiguration FilterBy { get; }

      /// <summary>
      /// Performance counters configuration
      /// </summary>
      IPerformanceCounterConfiguration CollectPerformanceCounters { get; }
   }
}
