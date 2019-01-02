using LogMagic.Enrichers;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;

namespace LogMagic.Microsoft.ServiceFabric.Writers
{
   class HealthReportWriter : ILogWriter
   {
      private readonly ServiceContext _context;

      public HealthReportWriter(ServiceContext context)
      {
         _context = context ?? throw new ArgumentNullException(nameof(context));
      }

      public void Write(IEnumerable<LogEvent> events)
      {
         foreach (LogEvent e in events)
         {
            object healthProperty = e.GetProperty(KnownProperty.ClusterHealthProperty);
            if (healthProperty == null) continue;

            var hi = new HealthInformation(e.SourceName, healthProperty.ToString(),
               e.ErrorException == null ? HealthState.Warning : HealthState.Error);
            hi.Description = e.Message;
            hi.TimeToLive = TimeSpan.FromMinutes(5);
            hi.RemoveWhenExpired = true;
            if (e.ErrorException != null)
            {
               hi.Description += Environment.NewLine;
               hi.Description += e.GetProperty(KnownProperty.Error).ToString();
            };

            _context.CodePackageActivationContext.ReportDeployedServicePackageHealth(hi);
         }
      }

      public void Dispose()
      {
         throw new NotImplementedException();
      }

   }
}
