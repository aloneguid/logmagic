using LogMagic.Enrichers;
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
         /*L.Config
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
               */

         L.Config
            .WriteTo.Trace()
            .WriteTo.Console()
            .WriteTo.PoshConsole()
            .WriteTo.AzureApplicationInsights("bd1cb207-a247-4db3-aa01-d512ed7d1f2a", flushOnWrite: true)
            .FilterBy.MinLogSeverity(LogSeverity.Verbose);

         log.Write("test1");

         log.Write("dividing...",
            "a", 5,
            "b", 7);

         log.Error("unexpected", new InvalidOperationException());

         log.Error("test", new NullReferenceException());

         using (log.Context("one", "two"))
         {
            log.Write("just a test");
         }

         log.Event("my ev");

         ApplicationMap();

         C.ReadKey();
      }

      private static void ApplicationMap()
      {
         using (log.Context(KnownProperty.OperationId, Guid.NewGuid().ToString()))
         {
            string webSiteActivityId = Guid.NewGuid().ToShortest();
            string serverActivityId = Guid.NewGuid().ToShortest();

            Exception ex = RandomGenerator.GetRandomInt(10) > 7 ? new Exception("simulated failure") : null;

            //---- web site
            using (log.Context(
               KnownProperty.RoleName, "Web Site"))
            {
               log.TrackUnknownIncomingRequest("LogIn", RandomDurationMs(500, 600), ex);

               log.Write("checking credentials on the server...");

               using (log.Context(KnownProperty.ActivityId, webSiteActivityId))
               {
                  log.TrackOutgoingRequest(webSiteActivityId, "Server", "CheckCredential", 100, null);
               }
            }

            //---- server
            using (log.Context(
               KnownProperty.RoleName, "Server",
               KnownProperty.ParentActivityId, webSiteActivityId,
               KnownProperty.ActivityId, serverActivityId))
            {
               log.TrackIncomingRequest(webSiteActivityId, serverActivityId, "CheckCredential", RandomDurationMs(400, 500), null);

               log.Write("fetching user from DB...");

               log.TrackOutgoingRequest(serverActivityId, "MSSQL", "GetUser", RandomDurationMs(100, 200), null);

               if(ex != null)
               {
                  log.Write("failed to fetch user", ex);
               }

               log.Write("fetching user picture");

               log.TrackOutgoingRequest(serverActivityId, "Blob Storage", "GetUserPicture", RandomDurationMs(100, 200), null);
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
