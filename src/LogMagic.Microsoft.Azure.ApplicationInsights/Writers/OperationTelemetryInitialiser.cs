﻿using System;
using System.Collections.Generic;
using System.Text;
using LogMagic.Enrichers;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace LogMagic.Microsoft.Azure.ApplicationInsights.Writers
{
   class OperationTelemetryInitialiser : ITelemetryInitializer
   {
      public static string Version { get; set; }

      public static string RoleName { get; set; }

      public static string RoleInstance { get; set; }

      public void Initialize(ITelemetry telemetry)
      {
         telemetry.Context.Component.Version = Version;
         telemetry.Context.Cloud.RoleInstance = RoleInstance;
         telemetry.Context.Cloud.RoleName = RoleName;

#if !NET45
         string operationId = L.GetContextValue(KnownProperty.OperationId);
         telemetry.Context.Operation.Id = operationId;
#endif
      }
   }
}
