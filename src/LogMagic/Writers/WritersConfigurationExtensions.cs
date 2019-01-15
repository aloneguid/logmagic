using System;
using LogMagic.Writers;

namespace LogMagic
{
   /// <summary>
   /// Extensions methods to help initialise configuration
   /// </summary>
   public static class WritersExtensions
   {
      /// <summary>
      /// Writes to system console
      /// </summary>
      public static ILogConfiguration AddConsole(this ILogConfiguration configuration)
      {
         return configuration.AddWriter(new ConsoleLogWriter(null));
      }

      /// <summary>
      /// Writes to system console
      /// </summary>
      public static ILogConfiguration AddConsole(this ILogConfiguration configuration, string format)
      {
         return configuration.AddWriter(new ConsoleLogWriter(format));
      }

      /// <summary>
      /// Writes to posh system console i.e. with nice colours
      /// </summary>
      public static ILogConfiguration AddPoshConsole(this ILogConfiguration configuration)
      {
         return configuration.AddWriter(new PoshConsoleLogWriter(null));
      }

      /// <summary>
      /// Writes to posh system console i.e. with nice colours
      /// </summary>
      public static ILogConfiguration AddPoshConsole(this ILogConfiguration configuration, string format)
      {
         return configuration.AddWriter(new PoshConsoleLogWriter(format));
      }

      /// <summary>
      /// Writes to .NET trace
      /// </summary>
      public static ILogConfiguration AddTrace(this ILogConfiguration configuration)
      {
         return configuration.AddWriter(new TraceLogWriter(null));
      }

      /// <summary>
      /// Writes to .NET trace
      /// </summary>
      public static ILogConfiguration AddTrace(this ILogConfiguration configuration, string format)
      {
         return configuration.AddWriter(new TraceLogWriter(format));
      }

      /// <summary>
      /// Writes to file on disk and rolls it over every day.
      /// </summary>
      public static ILogConfiguration AddFile(this ILogConfiguration configuration, string fileName)
      {
         return configuration.AddWriter(new FileLogWriter(fileName, null));
      }

      /// <summary>
      /// Writes to file on disk and rolls it over every day.
      /// </summary>
      public static ILogConfiguration AddFile(this ILogConfiguration configuration, string fileName, string format)
      {
         return configuration.AddWriter(new FileLogWriter(fileName, format));
      }

      /// <summary>
      /// Writes events to Seq server (https://getseq.net/)
      /// </summary>
      public static ILogConfiguration AddSeq(this ILogConfiguration configuration, Uri serverAddress)
      {
         return configuration.AddWriter(new SeqWriter(serverAddress, null));
      }

      /// <summary>
      /// Writes events to Seq server (https://getseq.net/)
      /// </summary>
      public static ILogConfiguration AddSeq(this ILogConfiguration configuration, Uri serverAddress, string apiKey)
      {
         return configuration.AddWriter(new SeqWriter(serverAddress, apiKey));
      }
   }
}
