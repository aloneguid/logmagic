using System.Collections.Generic;
using LogMagic.Configuration;

namespace LogMagic
{
   /// <summary>
   /// Entry point to logging configuration
   /// </summary>
   public interface ILogConfiguration : ILevelConfiguration
   {
#if !NET45
      /// <summary>
      /// Logging context
      /// </summary>
      LogContext Context { get; }

#endif

      /// <summary>
      /// Removes all configured filters
      /// </summary>
      /// <returns></returns>
      ILogConfiguration ClearFilters();

      /// <summary>
      /// Removes all configured writers
      /// </summary>
      /// <returns></returns>
      ILogConfiguration ClearWriters();

      /// <summary>
      /// Removes all configured enrichers
      /// </summary>
      /// <returns></returns>
      ILogConfiguration ClearEnrichers();

      /// <summary>
      /// Gets the list of configured enrichers
      /// </summary>
      IEnumerable<IEnricher> Enrichers { get; }

      /// <summary>
      /// Gets the list of configured writers
      /// </summary>
      IEnumerable<ILogWriter> Writers { get; }

      /// <summary>
      /// Get available filters for a log writer
      /// </summary>
      /// <param name="writer"></param>
      /// <returns></returns>
      IReadOnlyCollection<IFilter> GetFilters(ILogWriter writer);

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
      IFilterConfiguration When { get; }
   }
}
