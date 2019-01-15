using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;
using LogMagic.Proxy;
using NetBox.Extensions;

namespace LogMagic
{
   /// <summary>
   /// 
   /// </summary>
   public delegate void OnBeforeMethodExecution(ILog log, MethodInfo method, object[] arguments);

   /// <summary>
   /// 
   /// </summary>
   public delegate void OnAfterMethodExecution(ILog log, MethodInfo method, object[] argument, object returnValue, long ticks, Exception error);

   /// <summary>
   /// Useful logging extensions
   /// </summary>
   public static class ILogExtensions
   {
      private static ProxyGenerator _castleProxyGenerator = new ProxyGenerator();

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
      /// Writes a message as an error, by adding <see cref="KnownProperty.Error"/> property.
      /// </summary>
      /// <param name="log"></param>
      /// <param name="message"></param>
      /// <param name="error"></param>
      /// <param name="parameters"></param>
      public static void Error(this ILog log, string message, Exception error, params object[] parameters)
      {
         IDictionary<string, object> ps = ToDictionary(false, parameters);
         ps[KnownProperty.Error] = error;

         log.Write(message, LogSeverity.Error, ps);
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

      #region [ Proxy Logging ]

      /// <summary>
      /// Creates a middle man proxy for a class that logs all the calls to this class. The class must implement an interface.
      /// </summary>
      /// <typeparam name="TInterface">Interface that this class implements</typeparam>
      /// <typeparam name="TImplementation">Class intance</typeparam>
      /// <param name="log"></param>
      /// <param name="instance"></param>
      /// <param name="logExceptions"></param>
      /// <param name="onBeforeExecution"></param>
      /// <param name="onAfterExecution"></param>
      /// <returns></returns>
      public static TInterface CreateInterfaceLogger<TInterface, TImplementation>(this ILog log, TImplementation instance,
         bool logExceptions = true,
         OnBeforeMethodExecution onBeforeExecution = null,
         OnAfterMethodExecution onAfterExecution = null)
         where TImplementation : TInterface
      {
         if (instance == null) throw new ArgumentNullException(nameof(instance));

         var interceptor = new LoggingInterceptor(log, logExceptions, onBeforeExecution, onAfterExecution);

         return (TInterface)_castleProxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), instance, interceptor);
      }

      #endregion


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
