﻿using LogMagic.Enrichers;
using LogMagic.Microsoft.Azure.ApplicationInsights;
using NetBox.Extensions;
using NetBox.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using C = System.Console;

namespace LogMagic.Console
{
   public static class Program
   {
      private static readonly ILog log = L.G(typeof(Program));

      public static void Main(string[] args)
      {
         //initialise
         L.Config
            .WriteTo.PoshConsole()
            .WriteTo.Console()
            //.WriteTo.WindowsSpeech("fuck!", true)
            .WriteTo.AzureApplicationInsights("13d9faf0-e96d-46ce-81b1-d8303c798765",
               new WriterOptions
               {
                  FlushOnWrite = false,
                  EnableQuickPulse = true
               })
            .CollectPerformanceCounters.PlatformDefault()
            .CollectPerformanceCounters
               .WindowsCounter("Machine CPU Load (%)", "Processor", "% Processor Time", "_Total")
            .CollectPerformanceCounters
               .WithSamplingInterval(TimeSpan.FromSeconds(10));

         log.Trace("unexpected", new InvalidOperationException());

         log.Trace("test", new NullReferenceException());

         using (L.Context("one", "two"))
         {
            log.Trace("just a test");
         }

         C.ReadKey();

         /*while (true)
         {
            //Basics(5);

            ApplicationMap();

            Thread.Sleep(TimeSpan.FromSeconds(1));
         }*/
      }

      private static void Basics(int maxObjects)
      {
         using (L.Context(
            "Scenario", "Basics",
            KnownProperty.OperationId, Guid.NewGuid().ToString()))
         {
            using (var time = new TimeMeasure())
            {
               log.Event("Create Started");

               for (int i = 0; i < maxObjects; i++)
               {
                  log.Trace("creating object {0}", i);

                  log.Metric("Process Working Set", Environment.WorkingSet);
               }

               log.Event("Create Finished",
                  "Objects Created", maxObjects);

               log.Request("Create Objects", time.ElapsedTicks, null,
                  "Objects Created", maxObjects);
            }
         }
      }

      private static void ApplicationMap()
      {
         using (L.Context(KnownProperty.OperationId, Guid.NewGuid().ToString()))
         {
            string webSiteActivityId = Guid.NewGuid().ToShortest();
            string serverActivityId = Guid.NewGuid().ToShortest();

            Exception ex = RandomGenerator.GetRandomInt(10) > 7 ? new Exception("simulated failure") : null;

            //---- web site
            using (L.Context(
               KnownProperty.RoleName, "Web Site"))
            {
               log.Request("LogIn", RandomDurationMs(500, 600), ex);

               log.Trace("checking credentials on the server...");

               using (L.Context(KnownProperty.ActivityId, webSiteActivityId))
               {
                  log.Dependency("Server", "Server", "CheckCredential", 100);
               }
            }

            //---- server
            using (L.Context(
               KnownProperty.RoleName, "Server",
               KnownProperty.ParentActivityId, webSiteActivityId,
               KnownProperty.ActivityId, serverActivityId))
            {
               log.Request("CheckCredential", RandomDurationMs(400, 500));

               log.Trace("fetching user from DB...");

               log.Dependency("Databases", "MSSQL", "GetUser", RandomDurationMs(100, 200), ex);

               if(ex != null)
               {
                  log.Trace("failed to fetch user", ex);
               }

               log.Trace("fetching user picture");

               log.Dependency("Blob Storage", "Primary", "GetUserPicture", RandomDurationMs(100, 200));
            }

         }
      }

      private static long RandomDurationMs(int min, int max)
      {
         int ms = RandomGenerator.GetRandomInt(min, max);

         return TimeSpan.FromMilliseconds(ms).Ticks;
      }
   }
}
