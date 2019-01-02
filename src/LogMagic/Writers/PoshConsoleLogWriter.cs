using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogMagic.Enrichers;
using LogMagic.Tokenisation;

namespace LogMagic.Writers
{
   /// <summary>
   /// Posh colorful console, you should use this if you want to look cool.
   /// </summary>
   class PoshConsoleLogWriter : ILogWriter
   {
      private static readonly ConcurrentDictionary<string, string> SourceNameToShortName = new ConcurrentDictionary<string, string>();
      private static object ColourLock = new object();
      private readonly FormattedString _format;
      private const ConsoleColor SeparatorColour = ConsoleColor.DarkGray,
                                 SourceColour = ConsoleColor.DarkGray,
                                 MessageColour = ConsoleColor.White,
                                 ParameterColour = ConsoleColor.Green,
                                 ExceptionHeadlineColour = ConsoleColor.Red,
                                 ExceptionStackTraceColour = ConsoleColor.Gray,
                                 TimeColour = ConsoleColor.Green;
      private const string Pipe = "|";

      /// <summary>
      /// Receiver's settings
      /// </summary>
      public PoshConsoleLogWriterSettings Settings { get; private set; }

      private int _classNamePadding = 10;

      /// <summary>
      /// Constructs and instance of this class
      /// </summary>
      public PoshConsoleLogWriter(string format)
      {
         _format = format == null ? TextFormatter.DefaultFormat : FormattedString.Parse(format, null);

         Settings = new PoshConsoleLogWriterSettings
         {
            AbbreviateClassNames = false
         };

         Console.BackgroundColor = ConsoleColor.Black;
      }

      /// <summary>
      /// Sends the chunk to posh console!
      /// </summary>
      private void Write(LogEvent e)
      {
         foreach (Token token in _format.Tokens)
         {
            switch (token.Type)
            {
               case TokenType.String:
                  Cg.Write(token.Value, SeparatorColour);
                  break;
               case TokenType.Parameter:
                  switch (token.Name)
                  {
                     case TextFormatter.Time:
                        Cg.Write(e.EventTime.ToString(token.Format), TimeColour);
                        break;
                     case TextFormatter.Severity:
                        WriteSeverity(e);
                        break;
                     case TextFormatter.Source:
                        Cg.Write(e.SourceName, SourceColour);
                        break;
                     case TextFormatter.Message:
                        LogMessage(e);
                        break;
                     case TextFormatter.Error:
                        LogError(e);
                        break;
                     case TextFormatter.NewLine:
                        Console.WriteLine();
                        break;
                     case TextFormatter.Parameters:
                        LogProperties(e);
                        break;
                     default:
                        if (e.Properties != null)
                        {
                           object value;
                           if (e.Properties.TryGetValue(token.Name, out value))
                           {
                              string custom = _format.Format(token, value);
                              Cg.Write(custom, ParameterColour);
                           }
                        }
                        break;
                  }
                  break;
            }
         }

         Console.WriteLine();
      }

      private void LogProperties(LogEvent e)
      {
         if (e.Properties == null || e.Properties.Count == 0)
         {
            return;
         }

         int longestPropName = e.Properties.Keys.Max(k => k.Length);
         foreach(KeyValuePair<string, object> prop in e.Properties.Where(p => !TextFormatter.DoNotPrint(p.Key)))
         {
            Console.WriteLine();
            Console.Write("  |");
            string name = prop.Key.PadLeft(longestPropName);
            Cg.Write(name, ParameterColour);
            Console.Write(": ");
            Cg.Write(prop.Value?.ToString(), MessageColour);
         }
      }

      private void LogError(LogEvent e)
      {
         Exception ex = e.ErrorException;
         if (ex != null)
         {
            Console.WriteLine();
            Cg.Write($"{ex.GetType()}: {ex.Message}", ExceptionHeadlineColour);

            Console.WriteLine();
            Cg.Write(ex.StackTrace, ExceptionStackTraceColour);
         }
      }

      private void LogMessage(LogEvent e)
      {
         Cg.Write(e.Message, MessageColour);
      }

      private void WriteSeverity(LogEvent e)
      {
         object sevObj = e.GetProperty(KnownProperty.Severity);

         if (sevObj is LogSeverity sev)
         {

            switch (sev)
            {
               case LogSeverity.Critical:
                  Cg.Write("CRT", ConsoleColor.White, ConsoleColor.Red);
                  break;
               case LogSeverity.Error:
                  Cg.Write("ERR", ConsoleColor.White, ConsoleColor.Red);
                  break;
               case LogSeverity.Information:
                  Cg.Write("INF", ConsoleColor.White, ConsoleColor.Red);
                  break;
               case LogSeverity.Verbose:
                  Cg.Write("DBG", ConsoleColor.White, ConsoleColor.Red);
                  break;
               case LogSeverity.Warning:
                  Cg.Write("WRN", ConsoleColor.White, ConsoleColor.Red);
                  break;
               default:
                  Cg.Write("INF", ConsoleColor.White, ConsoleColor.DarkGreen);
                  break;
            }
         }
         else
         {
            if (e.ErrorException == null)
            {
               Cg.Write("INF", ConsoleColor.White, ConsoleColor.DarkGreen);
            }
            else
            {
               Cg.Write("ERR", ConsoleColor.White, ConsoleColor.Red);
            }
         }
      }

      private string Abbreviate(string sourceName)
      {
         if(!Settings.AbbreviateClassNames)
         {
            if(_classNamePadding < sourceName.Length) _classNamePadding = sourceName.Length;
            return sourceName.PadRight(_classNamePadding);
         }


         //the result here is padded
         string result;
         if(SourceNameToShortName.TryGetValue(sourceName, out result)) return result;

         string abbreviated = new string(sourceName.Where(char.IsUpper).ToArray());
         if(_classNamePadding < abbreviated.Length) _classNamePadding = abbreviated.Length;
         abbreviated = abbreviated.PadRight(_classNamePadding);
         SourceNameToShortName[sourceName] = abbreviated;
         return abbreviated;
      }

      private class Cg : IDisposable
      {
         private readonly ConsoleColor _prevForeground;
         private readonly ConsoleColor? _prevBackground;

         private Cg(ConsoleColor foreground, ConsoleColor? background = null)
         {
            _prevForeground = Console.ForegroundColor;
            Console.ForegroundColor = foreground;

            if(background != null)
            {
               _prevBackground = Console.BackgroundColor; ;
               Console.BackgroundColor = background.Value;
            }
         }

         public static void Write(string value, ConsoleColor color, ConsoleColor? background = null)
         {
            using(new Cg(color, background))
            {
               Console.Write(value);
            }
         }

         public void Dispose()
         {
            Console.ForegroundColor = _prevForeground;
            if(_prevBackground != null)
            {
               Console.BackgroundColor = _prevBackground.Value;
            }
         }

      }

      /// <summary>
      /// Dispose
      /// </summary>
      public void Dispose()
      {
         //nothing to dispose in posh console
      }

      public void Write(IEnumerable<LogEvent> events)
      {
         lock (ColourLock)
         {
            foreach (LogEvent e in events)
            {
               Write(e);
            }
         }
      }

      public Task WriteAsync(IEnumerable<LogEvent> events)
      {
         Write(events);
         return Task.FromResult(true);
      }
   }
}
