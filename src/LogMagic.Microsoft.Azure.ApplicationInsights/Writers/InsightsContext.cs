using LogMagic.Enrichers;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using NetBox.Extensions;
using System;
using System.Linq;

namespace LogMagic.Microsoft.Azure.ApplicationInsights.Writers
{
   class InsightsContext
   {
      private readonly TelemetryClient _client;
      private readonly WriterOptions _options;
      private readonly TelemetryContext _context;

      public InsightsContext(TelemetryClient client, WriterOptions options)
      {
         _client = client;
         _options = options;
         _context = client.Context;
      }

      public void Apply(LogEvent e)
      {
         OperationTelemetryInitialiser.Version = e.UseProperty(KnownProperty.Version, string.Empty);
         OperationTelemetryInitialiser.RoleName = e.UseProperty(KnownProperty.RoleName, string.Empty);
         OperationTelemetryInitialiser.RoleInstance = e.UseProperty(KnownProperty.RoleInstance, string.Empty);

         if(e.HasProperty(KnownProperty.EventName))
         {
            ApplyEvent(e);
         }
         else if(e.HasProperty(KnownProperty.DependencyName))
         {
            ApplyDependency(e);
         }
         else if(e.HasProperty(KnownProperty.RequestName))
         {
            ApplyRequest(e);
         }
         else if(e.HasProperty(KnownProperty.MetricName))
         {
            ApplyMetric(e);
         }
         else
         {
            ApplyTrace(e);
         }
      }

      private void ApplyDependency(LogEvent e)
      {
         var d = new DependencyTelemetry()
         {
            Type = e.UseProperty<string>(KnownProperty.DependencyType),
            Name = e.UseProperty<string>(KnownProperty.DependencyName),
            Data = e.UseProperty<string>(KnownProperty.DependencyCommand),
            Target = e.UseProperty<string>(KnownProperty.DependencyTarget),
            Duration = TimeSpan.FromTicks(e.UseProperty<long>(KnownProperty.Duration)),
            Success = e.ErrorException == null,
         };
         string id = e.UseProperty<string>(KnownProperty.ActivityId);
         if (id != null) d.Id = id;
         AddProperties(d, e);
         Add(d, e);

         _client.TrackDependency(d);
      }

      private void ApplyEvent(LogEvent e)
      {
         var t = new EventTelemetry
         {
            Name = e.UseProperty<string>(KnownProperty.EventName)
         };
         Add(t, e);
         AddProperties(t, e);

         _client.TrackEvent(t);
      }

      private void ApplyTrace(LogEvent e)
      {
         var tr = new TraceTelemetry(e.Message, GetSeverityLevel(e));
         Add(tr, e);
         AddProperties(tr, e);


         if (e.ErrorException != null)
         {
            if(_options.TraceExceptions)
            {
               _client.TrackTrace(tr);
            }

            var et = new ExceptionTelemetry(e.ErrorException);
            et.Message = e.Message;

            Add(et, e);
            AddProperties(et, e);

            _client.TrackException(et);
         }
         else
         {
            _client.TrackTrace(tr);
         }
      }

      private void ApplyRequest(LogEvent e)
      {
         string name = e.UseProperty<string>(KnownProperty.RequestName);
         string uri = e.UseProperty<string>(KnownProperty.RequestUri);
         string responseCode = GetHttpResponseCode(e);

         var tr = new RequestTelemetry
         {
            Name = name,
            Url = uri == null ? null : new Uri(uri),
            Duration = TimeSpan.FromTicks(e.UseProperty<long>(KnownProperty.Duration)),
            Success = e.ErrorException == null,
            ResponseCode = responseCode
         };
         string id = e.UseProperty<string>(KnownProperty.ActivityId);
         if (id != null) tr.Id = id;
         Add(tr, e);
         AddProperties(tr, e);

         _client.TrackRequest(tr);
      }

      private string GetHttpResponseCode(LogEvent e)
      {
         const string okCode = "200";
         const string badCode = "500";

         string setCode = e.UseProperty<string>(KnownProperty.ResponseCode);
         string exCode = e.ErrorException?.GetType().Name;

         if(setCode == okCode)
         {
            //sometimes response code is 200 but an exception is thrown, therefore we need to override it with 500
            if(exCode != null)
            {
               return badCode;
            }
         }

         return setCode;
      }

      private void ApplyMetric(LogEvent e)
      {
         var t = new MetricTelemetry();
         t.Name = e.UseProperty<string>(KnownProperty.MetricName);
         t.Sum = e.UseProperty<double>(KnownProperty.MetricValue);
         Add(t, e);
         AddProperties(t, e);

         _client.TrackMetric(t);
      }

      private static void AddProperties(ISupportProperties telemetry, LogEvent e)
      {
         telemetry.Properties.Add("loggerName", e.SourceName);

         if (e.Properties == null) return;

         telemetry.Properties.AddRange(e.Properties.ToDictionary(entry => entry.Key, entry => entry.Value?.ToString()));
      }

      private static void Add(ITelemetry telemetry, LogEvent e)
      {
         telemetry.Timestamp = e.EventTime;
      }

      private static SeverityLevel GetSeverityLevel(LogEvent e)
      {
         LogSeverity sev = e.UseProperty(KnownProperty.Severity, LogSeverity.Information);

         switch (sev)
         {
            case LogSeverity.Verbose:
               return SeverityLevel.Verbose;
            case LogSeverity.Information:
               return SeverityLevel.Information;
            case LogSeverity.Warning:
               return SeverityLevel.Warning;
            case LogSeverity.Error:
               return SeverityLevel.Critical;
            case LogSeverity.Critical:
               return SeverityLevel.Critical;
            default:
               return SeverityLevel.Information;
         }
      }
   }
}
