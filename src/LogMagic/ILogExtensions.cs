using System;
using System.Collections.Generic;

namespace LogMagic
{
   /// <summary>
   /// Useful logging extensions
   /// </summary>
   public static class ILogExtensions
   {
      /// <summary>
      /// 
      /// </summary>
      /// <param name="log"></param>
      /// <param name="message"></param>
      /// <param name="parameters"></param>
      public static void Write(this ILog log, string message, params object[] parameters)
      {
         log.Write(message, ToDictionary(true, parameters));
      }

      /// <summary>
      /// Writes a message as an error, by adding <see cref="KnownProperty.Error"/> property and setting <see cref="KnownProperty.Severity"/> to <see cref="LogSeverity.Error"/>
      /// </summary>
      /// <param name="log"></param>
      /// <param name="message"></param>
      /// <param name="error"></param>
      /// <param name="parameters"></param>
      public static void Error(this ILog log, string message, Exception error, params object[] parameters)
      {
         IDictionary<string, object> ps = ToDictionary(false, parameters);
         ps[KnownProperty.Error] = error;
         ps[KnownProperty.Severity] = LogSeverity.Error;

         log.Write(message, ps);
      }

#if !NET45

      /// <summary>
      /// Creates a disposable logging context that enriches it with custom properties
      /// </summary>
      /// <param name="log"></param>
      /// <param name="parameters"></param>
      /// <returns></returns>
      public static IDisposable Context(this ILog log, params object[] parameters)
      {
         return log.Context(ToDictionary(true, parameters));
      }

      /// <summary>
      /// Gets context value
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="log"></param>
      /// <param name="propertyName"></param>
      /// <param name="defaultValue"></param>
      /// <returns></returns>
      public static T GetContextValue<T>(this ILog log, string propertyName, T defaultValue = default(T))
      {
         object result = log.GetContextValue(propertyName);
         if (result is T) return (T)result;
         return defaultValue;
      }

#endif

      /// <summary>
      /// 
      /// </summary>
      /// <param name="log"></param>
      /// <param name="name"></param>
      /// <param name="value"></param>
      public static void Metric(this ILog log, string name, double value)
      {
         log.Write(null,
            KnownProperty.MetricName, name,
            KnownProperty.MetricValue, value);
      }

      //-------------------------- old code

      /// <summary>
      /// Track dependency
      /// </summary>
      public static void Dependency(this ILog log, string type, string name, string command, long duration,
         Exception error = null,
         params object[] properties)
      {
         log.Dependency(type, name, command, duration, error, ToDictionary(true, properties));
      }

      /// <summary>
      /// Track dependency
      /// </summary>
      public static void Dependency(this ILog log, string name, string command, long duration,
         Exception error,
         params object[] properties)
      {
         log.Dependency(name, name, command, duration, error, ToDictionary(true, properties));
      }

      /// <summary>
      /// Track request
      /// </summary>
      public static void Request(this ILog log, string name, long duration, Exception error = null, params object[] properties)
      {
         log.Request(name, duration, error, ToDictionary(true, properties));
      }

      /// <summary>
      /// Track event
      /// </summary>
      public static void Event(this ILog log, string name, params object[] properties)
      {
         log.Event(name, ToDictionary(true, properties));
      }

      private static IDictionary<string, object> ToDictionary(bool allowNullResult, params object[] properties)
      {
         if ((properties == null || properties.Length == 0) && allowNullResult) return null;

         var d = new Dictionary<string, object>(properties.Length);

         int maxLength = properties.Length - properties.Length % 2;
         for (int i = 0; i < maxLength; i += 2)
         {
            object keyObj = properties[i];
            if (!(keyObj is string key)) throw new ArgumentOutOfRangeException($"{nameof(properties)}[{i}]", "parameter must be of string type as it's meant to be a parameter name");

            d[key] = properties[i + 1];
         }

         return d;
      }
   }
}
