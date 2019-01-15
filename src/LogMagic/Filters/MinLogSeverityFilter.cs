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

         return e.Severity >= _minSeverity;
      }
   }
}
