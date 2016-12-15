﻿using LogMagic.Writers;

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
         return configuration.Custom(new ConsoleLogWriter(null));
      }

      /// <summary>
      /// Writes to system console
      /// </summary>
      public static ILogConfiguration Console(this IWriterConfiguration configuration, string format)
      {
         return configuration.Custom(new ConsoleLogWriter(format));
      }

      /// <summary>
      /// Writes to posh system console i.e. with nice colours
      /// </summary>
      public static ILogConfiguration PoshConsole(this IWriterConfiguration configuration)
      {
         return configuration.Custom(new PoshConsoleLogWriter(null));
      }

      /// <summary>
      /// Writes to posh system console i.e. with nice colours
      /// </summary>
      public static ILogConfiguration PoshConsole(this IWriterConfiguration configuration, string format)
      {
         return configuration.Custom(new PoshConsoleLogWriter(format));
      }

      /// <summary>
      /// Writes to .NET trace
      /// </summary>
      public static ILogConfiguration Trace(this IWriterConfiguration configuration)
      {
         return configuration.Custom(new TraceLogWriter(null));
      }

      /// <summary>
      /// Writes to .NET trace
      /// </summary>
      public static ILogConfiguration Trace(this IWriterConfiguration configuration, string format)
      {
         return configuration.Custom(new TraceLogWriter(format));
      }

      /// <summary>
      /// Writes to file on disk
      /// </summary>
      public static ILogConfiguration File(this IWriterConfiguration configuration, string fileName)
      {
         return configuration.Custom(new FileLogWriter(fileName, null));
      }

      /// <summary>
      /// Writes to file on disk
      /// </summary>
      public static ILogConfiguration File(this IWriterConfiguration configuration, string fileName, string format)
      {
         return configuration.Custom(new FileLogWriter(fileName, format));
      }
   }
}
