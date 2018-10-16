﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogMagic;
using LogMagic.Enrichers;
using Microsoft.Azure.WebJobs.Host;

namespace Microsoft.Azure.WebJobs
{
   public class LogMagicInvocationFilterAttribute : FunctionInvocationFilterAttribute
   {
      private static readonly ILog log = L.G(typeof(LogMagicInvocationFilterAttribute));

      static LogMagicInvocationFilterAttribute()
      {
         //we could configure AI automatically here, but it requires a reference to Application Insights library
         //string aiKey = 
      }

      public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
      {
         Dictionary<string, string> context = GetIncomingContext();

         using (L.Context(context))
         {
            using (var time = new TimeMeasure())
            {
               Exception gex = null;

               try
               {
                  await base.OnExecutingAsync(executingContext, cancellationToken);
               }
               catch(Exception ex)
               {
                  gex = ex;
                  throw;
               }
               finally
               {
                  using (L.Context(
                     "FunctionName", executingContext.FunctionName,
                     "FunctionInstanceId", executingContext.FunctionInstanceId.ToString()))
                  {
                     log.Request(executingContext.FunctionName, time.ElapsedTicks, gex);
                  }

               }
            }
         }
      }

      public override Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
      {
         return base.OnExecutedAsync(executedContext, cancellationToken);
      }

      private Dictionary<string, string> GetIncomingContext()
      {
         var result = new Dictionary<string, string>();

         //get root activity which is the one that initiated incoming call
         Activity rootActivity = Activity.Current;
         if (rootActivity != null)
         {
            while (rootActivity.Parent != null)
               rootActivity = rootActivity.Parent;

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

   }
}
