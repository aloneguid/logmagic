namespace LogMagic
{
   /// <summary>
   /// Log writers configuration
   /// </summary>
   public interface IWriterConfiguration
   {
      /// <summary>
      /// Use to add a custom writer
      /// </summary>
      /// <param name="writer"></param>
      /// <returns></returns>
      ILogConfiguration Writer(ILogWriter writer);
   }
}
