using System;
using System.Runtime.CompilerServices;
using LogMagic.Tokenisation;
using LogMagic.Enrichers;
using System.Collections.Generic;

namespace LogMagic
{
   class EventFactory
   {
      /// <summary>
      /// Creates a rich logging event
      /// </summary>
      /// <param name="sourceName"></param>
      /// <param name="message"></param>
      /// <param name="properties"></param>
      /// <returns></returns>
      [MethodImpl(MethodImplOptions.NoInlining)]
      public LogEvent CreateEvent(string sourceName, string message, IDictionary<string, object> properties)
      {
         var e = new LogEvent(sourceName, DateTime.UtcNow) { Message = message };

         if (properties != null && properties.Count > 0)
         {
            foreach (KeyValuePair<string, object> prop in properties)
            {
               e.AddProperty(prop.Key, prop.Value);
            }
         }

         //enrich
         Enrich(e, L.LogConfig.Enrichers);
         Enrich(e, LogContext.Enrichers?.Values);

         return e;
      }

      private void Enrich(LogEvent e, IEnumerable<IEnricher> enrichers)
      {
         if (enrichers == null) return;

         foreach (IEnricher enricher in enrichers)
         {
            string pn;
            object pv;
            enricher.Enrich(e, out pn, out pv);
            if (pn != null)
            {
               e.AddProperty(pn, pv);
            }
         }
      }

      private static Exception ExtractError(object[] parameters)
      {
         if (parameters != null && parameters.Length > 0)
         {
            Exception error = parameters[parameters.Length - 1] as Exception;
            if (error != null)
            {
               Array.Resize(ref parameters, parameters.Length - 1);
               return error;
            }
         }

         return null;
      }
   }
}
