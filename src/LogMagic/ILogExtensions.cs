using System;
using System.Collections.Generic;
using NetBox.Extensions;

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

      /// <summary>
      /// Track event
      /// </summary>
      public static void Event(this ILog log, string name, params object[] parameters)
      {
         IDictionary<string, object> ps = ToDictionary(false, parameters);
         ps[KnownProperty.EventName] = name;

         log.Write(null, ps);
      }

      /// <summary>
      /// Report metric
      /// </summary>
      public static void Metric(this ILog log, string name, double value)
      {
         log.Write(null,
            KnownProperty.MetricName, name,
            KnownProperty.MetricValue, value);
      }

      /// <summary>
      /// todo
      /// </summary>
      public static IDisposable TrackIncomingRequest(this ILog log,
         string requestName)
      {
         //request generates a new activity id, and pushes current activity id as it's parent

         //generate a new ID for this activity and swap them
         IDisposable context = log.Context(
            KnownProperty.ApplicationParentActivityId, log.GetContextValue(KnownProperty.ApplicationActivityId),
            KnownProperty.ApplicationActivityId, Guid.NewGuid().ToShortest());

         log.Write(null,
            KnownProperty.RequestName, requestName);

         return context;
      }

      /// <summary>
      /// todo
      /// </summary>
      public static IDisposable TrackOutgoingRequest(this ILog log,
         string targetType,
         string commandTemplate,
         string fullCommand)
      {
         //generate a new ID for this activity and swap them
         IDisposable context = log.Context(
            KnownProperty.ApplicationParentActivityId, log.GetContextValue(KnownProperty.ApplicationActivityId),
            KnownProperty.ApplicationActivityId, Guid.NewGuid().ToShortest());

         log.Write(null, 
            KnownProperty.DependencyType, targetType,
            KnownProperty.DependencyName, commandTemplate,
            KnownProperty.DependencyData, fullCommand);

         return context;
      }

      /// <summary>
      /// Helper method to create a context of a new operation
      /// </summary>
      public static IDisposable Operation(this ILog log)
      {
         return log.Context(KnownProperty.OperationId, Guid.NewGuid().ToShortest());
      }

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

      private static IDictionary<string, object> ToDictionary(bool allowNullResult, params object[] properties)
      {
         if ((properties == null || properties.Length == 0) && allowNullResult) return null;

         var d = new Dictionary<string, object>(properties.Length);

         int maxLength = properties.Length - properties.Length % 2;
         for (int i = 0; i < maxLength; i += 2)
         {
            object keyObj = properties[i];
            if (!(keyObj is string key)) throw new ArgumentOutOfRangeException($"{nameof(properties)}[{i}]", "parameter must be of string type as it's meant to be a parameter name");

            //don't add props with null values
            object value = properties[i + 1];
            if (value != null)
            {
               d[key] = value;
            }
         }

         return d;
      }
   }
}
