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
      /// <param name="properties">Optional properties</param>
      void Write(string message, IDictionary<string, object> properties = null);
   }
}
