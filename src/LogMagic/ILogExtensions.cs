using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
         log.Write(message, Compress(true, parameters));
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="log"></param>
      /// <param name="message"></param>
      /// <param name="error"></param>
      /// <param name="parameters"></param>
      public static void Error(this ILog log, string message, Exception error, params object[] parameters)
      {
         IDictionary<string, object> ps = Compress(false, parameters);
         ps[KnownProperty.Error] = error;

         log.Write(message, ps);
      }

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
         log.Dependency(type, name, command, duration, error, Compress(true, properties));
      }

      /// <summary>
      /// Track dependency
      /// </summary>
      public static void Dependency(this ILog log, string name, string command, long duration,
         Exception error,
         params object[] properties)
      {
         log.Dependency(name, name, command, duration, error, Compress(true, properties));
      }

      /// <summary>
      /// Track dependency with automation time measurement and error handling
      /// </summary>
      public static async Task<T> DependencyAsync<T>(this ILog log, string name, string command,
         Task<T> action,
         params object[] properties)
      {
         using (var time = new TimeMeasure())
         {
            try
            {
               T result = await action;

               log.Dependency(name, name, command, time.ElapsedTicks, null, properties);

               return result;
            }
            catch(Exception ex)
            {
               log.Dependency(name, name, command, time.ElapsedTicks, ex, properties);
               throw;
            }
         }
      }

      /// <summary>
      /// Track request
      /// </summary>
      public static void Request(this ILog log, string name, long duration, Exception error = null, params object[] properties)
      {
         log.Request(name, duration, error, Compress(true, properties));
      }

      /// <summary>
      /// Track event
      /// </summary>
      public static void Event(this ILog log, string name, params object[] properties)
      {
         log.Event(name, Compress(true, properties));
      }

      private static IDictionary<string, object> Compress(bool allowNullResult, params object[] properties)
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
