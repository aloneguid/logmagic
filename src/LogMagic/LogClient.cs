using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace LogMagic
{
   /// <summary>
   /// Utility class to server log clients
   /// </summary>
   class LogClient : ILog
   {
      private readonly ILogConfiguration _config;
      private readonly string _name;
      private readonly EventFactory _factory;
      private ILogWriter[] _writers;
      private IFilter[] _filters;

      public LogClient(ILogConfiguration config, string name)
      {
         _config = config ?? throw new ArgumentNullException(nameof(config));
         _name = name ?? throw new ArgumentNullException(nameof(name));
         _factory = new EventFactory();
      }

      public void Write(string message, IDictionary<string, object> properties)
      {
         Serve(message, properties);
      }

#if !NET45

      public IDisposable Context(IDictionary<string, object> properties)
      {
         if (properties == null || properties.Count == 0) return null;

         return LogContext.Push(properties);
      }

      public object GetContextValue(string propertyName)
      {
         if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

         return LogContext.GetValueByName(propertyName);
      }

      /// <summary>
      /// Gets a dictionary of all current context values
      /// </summary>
      /// <returns></returns>
      public IDictionary<string, object> GetContextValues()
      {
         return LogContext.GetAllValues();
      }

#endif


      internal void Serve(
         string message,
         IDictionary<string, object> properties)
      {
         LogEvent e = _factory.CreateEvent(_name, message, properties);

         SubmitNow(e);
      }

      private void SubmitNow(LogEvent e)
      {
         if(_writers == null)
         {
            _writers = L.LogConfig.Writers.ToArray();
            _filters = L.LogConfig.Filters.ToArray();
         }

         bool active = _filters.Length == 0 || _filters.Any(f => f.Match(e));

         if (!active) return;

         foreach (ILogWriter writer in _writers)
         {
            try
            {
               writer.Write(new[] { e });
            }
            catch(Exception ex)
            {
               //there is nowhere else to log the error as we are the logger!
               Trace.TraceError("could not write: " + ex);
            }
         }
      }


      /*[MethodImpl(MethodImplOptions.NoInlining)]
      public void Dependency(string type, string name, string command, long duration, Exception error, Dictionary<string, object> properties)
      {
         if (properties == null) properties = new Dictionary<string, object>();

         properties[KnownProperty.Duration] = duration;
         properties[KnownProperty.DependencyName] = name;
         properties[KnownProperty.DependencyType] = type;
         properties[KnownProperty.DependencyCommand] = command;

         var parameters = new List<object> { _name, command, TimeSpan.FromTicks(duration) };
         if (error != null) parameters.Add(error);

         Serve(EventType.Dependency, properties,
            "dependency {0}.{1} took {2}",
            parameters.ToArray());
      }

      [MethodImpl(MethodImplOptions.NoInlining)]
      public void Event(string name, Dictionary<string, object> properties)
      {
         if (properties == null) properties = new Dictionary<string, object>();
         properties[KnownProperty.EventName] = name;

         Serve(EventType.ApplicationEvent, properties,
            "application event {0} occurred",
            name);
      }

      [MethodImpl(MethodImplOptions.NoInlining)]
      public void Request(string name, long duration, Exception error, Dictionary<string, object> properties)
      {
         if (properties == null) properties = new Dictionary<string, object>();
         properties[KnownProperty.Duration] = duration;
         properties[KnownProperty.RequestName] = name;

         if (error != null)
         {
            properties[KnownProperty.Error] = error;
         }

         Serve(EventType.HandledRequest, properties,
            "request {0} took {1}", name, TimeSpan.FromTicks(duration));
      }

      [MethodImpl(MethodImplOptions.NoInlining)]
      public void Metric(string name, double value, Dictionary<string, object> properties)
      {
         if (properties == null) properties = new Dictionary<string, object>();

         properties[KnownProperty.MetricName] = name;
         properties[KnownProperty.MetricValue] = value;

         Serve(EventType.Metric, properties,
            "metric {0} == {1}",
            name, value);
      }
      */

      public override string ToString()
      {
         return _name;
      }
   }
}
