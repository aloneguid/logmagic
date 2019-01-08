using LogMagic.Enrichers;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using NetBox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogMagic.Microsoft.Azure.ApplicationInsights.Writers
{
   class InsightsContext
   {
      private readonly TelemetryClient _client;
      private readonly WriterOptions _options;
      private readonly TelemetryContext _context;

      //properties that shouldn't be attached explicitly
      private static readonly HashSet<string> _cleanupProperties = new HashSet<string>
      {
         KnownProperty.OperationId,
         KnownProperty.ApplicationActivityId,
         KnownProperty.ApplicationParentActivityId
      };

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

      private void ApplyRequest(LogEvent e)
      {
         string name = e.UseProperty<string>(KnownProperty.RequestName);
         string uri = e.UseProperty<string>(KnownProperty.RequestUri);
         string responseCode = e.UseProperty<string>(KnownProperty.ResponseCode) ?? GetHttpResponseCode(e);

         var tr = new RequestTelemetry
         {
            Id = e.GetProperty<string>(KnownProperty.ApplicationActivityId),
            Name = name,
            Url = uri == null ? null : new Uri(uri),
            Duration = TimeSpan.FromTicks(e.UseProperty<long>(KnownProperty.Duration)),
            Success = e.ErrorException == null,
            ResponseCode = responseCode,

            //Source = 
         };

         Init(tr, e);

         //override parent for this request
         tr.Context.Operation.ParentId =
            e.GetProperty<string>(KnownProperty.ApplicationParentActivityId)
            ?? e.GetProperty<string>(KnownProperty.OperationId);  //in case parent activity is missing

         _client.TrackRequest(tr);
      }

      private void ApplyDependency(LogEvent e)
      {
         var d = new DependencyTelemetry()
         {
            Id = e.GetProperty<string>(KnownProperty.ApplicationActivityId),

            // "SQL", "Azure Table", "HTTP" etc.
            Type = e.UseProperty<string>(KnownProperty.DependencyType),

            // generic name i.e. stored procedure name or URL _template_
            Name = e.UseProperty<string>(KnownProperty.DependencyName),

            // actual command, i.e. sql statement or _full_ URL including all querty parameters
            Data = e.UseProperty<string>(KnownProperty.DependencyData),

            //Target = e.UseProperty<string>(KnownProperty.DependencyTarget),

            Duration = TimeSpan.FromTicks(e.UseProperty<long>(KnownProperty.Duration)),
            Success = e.ErrorException == null,
         };

         Init(d, e);

         //override parent for this request
         d.Context.Operation.ParentId =
            e.GetProperty<string>(KnownProperty.ApplicationParentActivityId)
            ?? e.GetProperty<string>(KnownProperty.OperationId);  //in case parent activity is missing

         _client.TrackDependency(d);
      }

      private void ApplyEvent(LogEvent e)
      {
         var t = new EventTelemetry
         {
            Name = e.UseProperty<string>(KnownProperty.EventName)
         };
         Init(t, e);

         _client.TrackEvent(t);
      }

      private void ApplyTrace(LogEvent e)
      {
         if(e.ErrorException != null)
         {
            var et = new ExceptionTelemetry(e.ErrorException);
            et.Message = e.Message;
            et.Exception = e.ErrorException;

            Init(et, e);

            _client.TrackException(et);
         }
         else
         {
            var tr = new TraceTelemetry(e.Message, GetSeverityLevel(e));
            Init(tr, e);

            _client.TrackTrace(tr);
         }
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

         Init(t, e);

         _client.TrackMetric(t);
      }

      private static void Init<T>(T telemetry, LogEvent e) where T : ITelemetry, ISupportProperties
      {
         //ITelemetry
         telemetry.Context.Operation.Id = e.GetProperty(KnownProperty.OperationId, string.Empty);
         telemetry.Context.Operation.ParentId = e.GetProperty(KnownProperty.ApplicationActivityId, string.Empty);
         telemetry.Timestamp = e.EventTime;

         //ISupportProperties
         telemetry.Properties["source"] = e.SourceName;
         if (e.Properties != null)
         {
            Dictionary<string, string> toAdd = e.Properties
               .Where(p => !_cleanupProperties.Contains(p.Key))
               .ToDictionary(p => p.Key, p => p.Value?.ToString());

            if(toAdd.Count > 0)
            {
               telemetry.Properties.AddRange(toAdd);
            }
         }
      }

      private static SeverityLevel GetSeverityLevel(LogEvent e)
      {
         switch (e.Severity)
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
