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
      /// Track request that comes from an unknown source
      /// </summary>
      public static void TrackUnknownIncomingRequest(this ILog log,
         string requestName, long requestDuration,
         Exception error = null)
      {
         IDictionary<string, object> ps = new Dictionary<string, object>();

         ps[KnownProperty.RequestName] = requestName;
         ps[KnownProperty.Duration] = requestDuration;

         if(error != null)
         {
            ps[KnownProperty.Error] = error;
         }

         log.Write(null, ps);
      }

      /// <summary>
      /// 
      /// </summary>
      public static void TrackIncomingRequest(this ILog log,
         string callingActivityId,
         string activityId,
         string incomingOperationName,
         long totalRequestDuration,
         Exception error)
      {
         IDictionary<string, object> ps = new Dictionary<string, object>();

         ps[KnownProperty.RequestName] = incomingOperationName;
         ps[KnownProperty.Duration] = totalRequestDuration;
         ps[KnownProperty.ActivityId] = activityId;
         ps[KnownProperty.ParentActivityId] = callingActivityId;

         if (error != null)
         {
            ps[KnownProperty.Error] = error;
         }

         log.Write(null, ps);
      }

      /// <summary>
      /// Tracks outgoing request. This call needs to be made when your client has called a remote component that needs to be tracked.
      /// </summary>
      public static void TrackOutgoingRequest(this ILog log,
         string activityId,
         string remoteComponentName,
         string remoteOperationName,
         long outgoingRequestDurationMs,
         Exception error)
      {
         var ps = new Dictionary<string, object>();
         ps[KnownProperty.DependencyType] = remoteComponentName;
         ps[KnownProperty.DependencyName] = remoteComponentName;
         ps[KnownProperty.DependencyCommand] = remoteOperationName;
         ps[KnownProperty.Duration] = outgoingRequestDurationMs;

         if (error != null) ps[KnownProperty.Error] = error;

         log.Write(null, ps);
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
