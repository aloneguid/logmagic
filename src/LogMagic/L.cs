using System;

namespace LogMagic
{
   /// <summary>
   /// Global public logging configuration and initialisation class
   /// </summary>
   public static class L
   {
      private static LogConfiguration _config = new LogConfiguration();

      /// <summary>
      /// Gets logging library configuration
      /// </summary>
      public static ILogConfiguration Config => _config;

      internal static LogConfiguration LogConfig => _config;

      /// <summary>
      /// Get logger for the specified type
      /// <typeparam name="T">Class type</typeparam>
      /// </summary>
      public static ILog G<T>()
      {
         return new LogClient(Config, typeof(T).FullName);
      }

      /// <summary>
      /// Get logger for the specified type
      /// </summary>
      public static ILog G(Type t)
      {
         return new LogClient(Config, t.FullName);
      }

      /// <summary>
      /// Gets logger by specified name. Use when you can't use more specific methods.
      /// </summary>
      public static ILog G(string name)
      {
         return new LogClient(Config, name);
      }
   }
}
