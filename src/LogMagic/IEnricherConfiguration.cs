namespace LogMagic
{
   /// <summary>
   /// Configuration of logging enrichers
   /// </summary>
   public interface IEnricherConfiguration
   {
      /// <summary>
      /// Adds a custom enricher
      /// </summary>
      ILogConfiguration Enricher(IEnricher enricher);
   }
}
