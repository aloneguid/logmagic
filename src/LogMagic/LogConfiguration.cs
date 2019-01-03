using LogMagic.PerfCounters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogMagic
{
   class LogConfiguration : ILogConfiguration, IWriterConfiguration, IEnricherConfiguration, IFilterConfiguration, IPerformanceCounterConfiguration
   {
      private readonly List<ILogWriter> _writers = new List<ILogWriter>();
      private readonly List<IEnricher> _enrichers = new List<IEnricher>();
      private readonly List<IFilter> _filters = new List<IFilter>();
      private readonly List<IPerformanceCounter> _perfCounters = new List<IPerformanceCounter>();

      public IEnumerable<IEnricher> Enrichers => _enrichers;

      public IEnumerable<ILogWriter> Writers => _writers;

      public IEnumerable<IFilter> Filters => _filters;

      public IReadOnlyCollection<IPerformanceCounter> PerformanceCounters => _perfCounters;

      public ILogConfiguration Writer(ILogWriter writer)
      {
         _writers.Add(writer);

         return this;
      }

      public IWriterConfiguration WriteTo => this;

      public ILogConfiguration Enricher(IEnricher enricher)
      {
         _enrichers.Add(enricher);

         return this;
      }

      public IEnricherConfiguration EnrichWith => this;

      public ILogConfiguration Filter(IFilter filter)
      {
         _filters.Add(filter);

         return this;
      }

      public ILogConfiguration Custom(IPerformanceCounter performanceCounter)
      {
         if (performanceCounter != null)
         {
            _perfCounters.Add(performanceCounter);
         }

         return this;
      }

      public ILogConfiguration WithSamplingInterval(TimeSpan samplingInterval)
      {
         return this;
      }

      public void Reset()
      {
         _writers.Clear();
         _enrichers.Clear();
         _filters.Clear();
         _perfCounters.Clear();
      }

      public void Shutdown()
      {
         foreach(ILogWriter writer in _writers)
         {
            writer.Dispose();
         }
      }

      public IFilterConfiguration FilterBy => this;

      public IPerformanceCounterConfiguration CollectPerformanceCounters => this;
   }
}
