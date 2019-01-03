using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogMagic.Tokenisation
{
   /// <summary>
   /// Log formatting utility methods
   /// </summary>
   public static class TextFormatter
   {
      /// <summary>
      /// Default format string used to format log text
      /// </summary>
      public static readonly FormattedString DefaultFormat = FormattedString.Parse("{time:H:mm:ss,fff}|{level,-1}|{source}|{message}{parameters}{error}", null);

      static readonly FormattedString DefaultFormat2 = FormattedString.Parse(
         new FormattedStringBuilder()
            .AddTime("H:mm:ss,fff")
            .Build(),
         null
         );

      internal const string Time = "time";
      internal const string Severity = "level";
      internal const string Source = "source";
      internal const string Message = "message";
      internal const string Error = "error";
      internal const string NewLine = "br";
      internal const string Parameters = "parameters";

      /// <summary>
      /// Formats log event for text representation, not including any properties. Error is included though.
      /// </summary>
      public static string Format(LogEvent e, FormattedString format)
      {
         if (format == null) format = DefaultFormat;

         var b = new StringBuilder();

         foreach(Token token in format.Tokens)
         {
            switch(token.Type)
            {
               case TokenType.String:
                  b.Append(token.Value);
                  break;
               case TokenType.Parameter:
                  switch(token.Name)
                  {
                     case Time:
                        b.Append(e.EventTime.ToString(token.Format));
                        break;
                     case Severity:
                        string sev = ToSeverityString(e);
                        if (token.Format != null) sev = string.Format(token.NativeFormat, sev);
                        b.Append(sev);
                        break;
                     case Source:
                        b.Append(e.SourceName);
                        break;
                     case Message:
                        b.Append(e.Message);
                        break;
                     case Error:
                        if (e.ErrorException != null)
                        {
                           b.AppendLine();
                           b.Append(e.ErrorException.ToString());
                        }
                        break;
                     case Parameters:
                        AddParameters(b, e);
                        break;
                     case NewLine:
                        b.AppendLine();
                        break;
                     default:
                        if(e.Properties != null)
                        {
                           object value;
                           if(e.Properties.TryGetValue(token.Name, out value))
                           {
                              string custom = format.Format(token, value);
                              b.Append(custom);
                           }
                        }
                        break;
                  }
                  break;
            }
         }

         return b.ToString();
      }

      private static void AddParameters(StringBuilder b, LogEvent e)
      {
         if (e.Properties?.Count > 0)
         {
            int longestPropertyName = e.Properties.Max(p => p.Key.Length);

            IEnumerable<string> lines = e.Properties
               .Where(p => !TextFormatter.DoNotPrint(p.Key))
               .Select(p => $"  {p.Key.PadLeft(longestPropertyName)}: {p.Value}");

            foreach (string line in lines)
            {
               b.Append(Environment.NewLine);
               b.Append(line);
            }
         }
      }

      private static string ToSeverityString(LogEvent e)
      {
         switch(e.Severity)
         {
            case LogSeverity.Critical:
               return "C";
            case LogSeverity.Error:
               return "E";
            case LogSeverity.Information:
               return "I";
            case LogSeverity.Verbose:
               return "V";
            case LogSeverity.Warning:
               return "W";
            default:
               return "I";
         }
      }

      internal static bool DoNotPrint(string propertyName)
      {
         return propertyName == KnownProperty.Error;
      }
   }
}
