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
      public static ILogConfiguration Console(this IWriterConfiguration configuration)
      {
         return configuration.Writer(new ConsoleLogWriter(null));
      }

      /// <summary>
      /// Writes to system console
      /// </summary>
      public static ILogConfiguration Console(this IWriterConfiguration configuration, string format)
      {
         return configuration.Writer(new ConsoleLogWriter(format));
      }

      /// <summary>
      /// Writes to posh system console i.e. with nice colours
      /// </summary>
      public static ILogConfiguration PoshConsole(this IWriterConfiguration configuration)
      {
         return configuration.Writer(new PoshConsoleLogWriter(null));
      }

      /// <summary>
      /// Writes to posh system console i.e. with nice colours
      /// </summary>
      public static ILogConfiguration PoshConsole(this IWriterConfiguration configuration, string format)
      {
         return configuration.Writer(new PoshConsoleLogWriter(format));
      }

      /// <summary>
      /// Writes to .NET trace
      /// </summary>
      public static ILogConfiguration Trace(this IWriterConfiguration configuration)
      {
         return configuration.Writer(new TraceLogWriter(null));
      }

      /// <summary>
      /// Writes to .NET trace
      /// </summary>
      public static ILogConfiguration Trace(this IWriterConfiguration configuration, string format)
      {
         return configuration.Writer(new TraceLogWriter(format));
      }

      /// <summary>
      /// Writes to file on disk and rolls it over every day.
      /// </summary>
      public static ILogConfiguration File(this IWriterConfiguration configuration, string fileName)
      {
         return configuration.Writer(new FileLogWriter(fileName, null));
      }

      /// <summary>
      /// Writes to file on disk and rolls it over every day.
      /// </summary>
      public static ILogConfiguration File(this IWriterConfiguration configuration, string fileName, string format)
      {
         return configuration.Writer(new FileLogWriter(fileName, format));
      }

      /// <summary>
      /// Writes events to Seq server (https://getseq.net/)
      /// </summary>
      public static ILogConfiguration Seq(this IWriterConfiguration configuration, Uri serverAddress)
      {
         return configuration.Writer(new SeqWriter(serverAddress, null));
      }

      /// <summary>
      /// Writes events to Seq server (https://getseq.net/)
      /// </summary>
      public static ILogConfiguration Seq(this IWriterConfiguration configuration, Uri serverAddress, string apiKey)
      {
         return configuration.Writer(new SeqWriter(serverAddress, apiKey));
      }
   }
}
