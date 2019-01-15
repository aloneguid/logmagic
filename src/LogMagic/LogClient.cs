using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace LogMagic
{
   /// <summary>
   /// Utility class to serve log clients
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

      public void Write(LogSeverity severity, string message, IDictionary<string, object> properties)
      {
         Serve(message, severity, properties);
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
         LogSeverity severity,
         IDictionary<string, object> properties)
      {
         LogEvent e = _factory.CreateEvent(_name, message, properties);
         e.Severity = severity;

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

      public override string ToString()
      {
         return _name;
      }
   }
}
