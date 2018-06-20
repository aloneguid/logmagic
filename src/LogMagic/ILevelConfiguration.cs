namespace LogMagic.Configuration
{
   /// <summary>
   /// Gives the minmum logging configuration level for the current logging session
   /// </summary>
   public interface ILevelConfiguration
   {
      /// <summary>
      /// Sets the minimum logging severity to be logged above (defaults to verbose or Debug)
      /// </summary>
      LogSeverity LogLevel { get; set; }
   }
}