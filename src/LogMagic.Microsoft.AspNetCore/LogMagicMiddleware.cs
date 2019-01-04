using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LogMagic.Enrichers;
using Microsoft.AspNetCore.Http;
using NetBox.Extensions;

namespace LogMagic.Microsoft.AspNetCore
{
   class LogMagicMiddleware
   {
      private static readonly ILog log = L.G(typeof(LogMagicMiddleware));
      private readonly RequestDelegate _next;

      public LogMagicMiddleware(RequestDelegate next)
      {
         _next = next;
      }
      
      public async Task Invoke(HttpContext context)
      {
         string name = $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}";
         string uri = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";

         //parent activity ID is returned here
         Dictionary<string, string> correlationContext = GetIncomingContext();

         //everything happening inside this request will have a proper operation id and
         //parent activity id set from correlation context
         using (log.Context(correlationContext))
         {
            using (var time = new TimeMeasure())
            {
               Exception gex = null;

               try
               {
                  await _next(context);
               }
               catch (Exception ex)
               {
                  gex = ex;
                  throw;
               }
               finally
               {
                  string responseCode = context.Response.StatusCode.ToString();

                  // request will have a new ID but parentId is fetched from current context which links it appropriately
                  using (log.Context(
                     KnownProperty.RequestUri, uri,
                     KnownProperty.ResponseCode, responseCode))
                  {
                     log.TrackUnknownIncomingRequest(name, time.ElapsedTicks, gex);
                  }
               }
            }
         }
      }

      private Dictionary<string, string> GetIncomingContext()
      {
         var result = new Dictionary<string, string>();

         //get root activity which is the one that initiated incoming call
         Activity rootActivity = Activity.Current;
         if (rootActivity != null)
         {
            while (rootActivity.Parent != null) rootActivity = rootActivity.Parent;

            //add properties which are stored in baggage
            foreach (KeyValuePair<string, string> baggageItem in rootActivity.Baggage)
            {
               string key = baggageItem.Key;
               string value = baggageItem.Value;

               result[key] = value;
            }

            result[KnownProperty.ParentActivityId] = rootActivity.ParentId;
         }

         return result;
      }

      /*private bool TryGetFromAppInsightsContext(HttpContext context, out string operationId, out string operationParentId, out string telemetryId)
      {
         RequestTelemetry appInsightsTelemetry = context.Features.Get<RequestTelemetry>();

         if (appInsightsTelemetry != null)
         {
            if(!_appInsightsInitialised)
            {
               string roleName = appInsightsTelemetry.Context.Cloud?.RoleName;
               string roleInstance = appInsightsTelemetry.Context.Cloud?.RoleInstance;

               if (roleName != null) L.Config.EnrichWith.Constant(KnownProperty.RoleName, roleName);
               if (roleInstance != null) L.Config.EnrichWith.Constant(KnownProperty.RoleInstance, roleInstance);

               _appInsightsInitialised = true;
            }

            return appInsightsTelemetry.Context.Operation.Id;
         }

         return false;
      }*/

   }
}
