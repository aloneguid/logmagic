using System;
using System.Collections.Generic;

namespace LogMagic
{
   /// <summary>
   /// Logging interface used by the client code, the most high level
   /// </summary>
   public interface ILog
   {
      /// <summary>
      /// Writes a log message
      /// </summary>
      /// <param name="message">Log message</param>
      /// <param name="severity"></param>
      /// <param name="properties">Optional properties</param>
      void Write(string message, LogSeverity severity, IDictionary<string, object> properties = null);

#if !NET45

      /// <summary>
      /// Creates a disposable logging context that enriches it with custom properties
      /// </summary>
      /// <param name="properties"></param>
      /// <returns></returns>
      IDisposable Context(IDictionary<string, object> properties);

      /// <summary>
      /// Gets current context value by property name.
      /// </summary>
      /// <param name="propertyName"></param>
      /// <returns>Context value or null if context has not value of such property.</returns>
      object GetContextValue(string propertyName);

      /// <summary>
      /// Gets a dictionary of all current context values
      /// </summary>
      /// <returns></returns>
      IDictionary<string, object> GetContextValues();

#endif

   }
}
