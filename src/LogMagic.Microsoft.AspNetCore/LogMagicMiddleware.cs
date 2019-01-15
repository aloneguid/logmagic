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

         GetActivityIds(out string activityId, out string parentActivityId);

         using (log.Context(
            KnownProperty.RequestName, name,
            KnownProperty.RequestUri, uri,
            KnownProperty.ApplicationActivityId, activityId,
            KnownProperty.ApplicationParentActivityId, parentActivityId))
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

                  log.Write(LogSeverity.Information,
                     null,
                     KnownProperty.Duration, time.ElapsedTicks,
                     KnownProperty.ResponseCode, responseCode);
               }
            }
         }
      }

      private Dictionary<string, string> GetIncomingContext()
      {
         var result = new Dictionary<string, string>();

         //get root activity which is the one that initiated incoming call
         Activity activity = Activity.Current;
         if (activity != null)
         {
            //add properties which are stored in baggage
            foreach (KeyValuePair<string, string> baggageItem in activity.Baggage)
            {
               string key = baggageItem.Key;
               string value = baggageItem.Value;

               result[key] = value;
            }

            //ASP.NET uses Activity and pre-populates it with IDs, we can use them
            string activityId = activity.Id;
            string parentActivityId = activity.Parent?.Id;

            result[KnownProperty.ApplicationActivityId] = activityId;
            result[KnownProperty.ApplicationParentActivityId] = parentActivityId;
         }

         return result;
      }

      private void GetActivityIds(out string activityId, out string parentActivityId)
      {
         Activity activity = Activity.Current;
         if(activity == null)
         {
            activityId = parentActivityId = null;
            return;
         }


         activityId = activity.Id;
         parentActivityId = activity.ParentId;
      }
   }
}
