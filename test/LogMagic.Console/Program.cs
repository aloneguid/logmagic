﻿using System;
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
            .AddTrace()
            .AddConsole()
            .AddPoshConsole()
            .AddAzureApplicationInsights("bd1cb207-a247-4db3-aa01-d512ed7d1f2a", flushOnWrite: true)
            .FilterBy.MinLogSeverity(LogSeverity.Verbose)
            .EnrichWith.Constant(KnownProperty.RoleName, "console app")
            .EnrichWith.Constant(KnownProperty.RoleInstance, Guid.NewGuid().ToString());

         //start global operation
         using (log.Operation())
         {
            //pretent we've got a global request
            using (log.TrackIncomingRequest("launch"))
            {
               //basic logging
               log.Write(LogSeverity.Information, "simple write");

               log.Write(LogSeverity.Information, "dividing with parameters",
                  "a", 5,
                  "b", 7);

               log.Error("unexpected", new InvalidOperationException());

               using (log.Context("one", "two"))
               {
                  log.Write(LogSeverity.Information, "properties in context");
               }

               log.Event("custom event");

               //make a request to "twitter api"
               using (log.TrackOutgoingRequest("Twitter", "getTweets", "getTweets?type=mine"))
               {
                  //pretend we are the twitter API and accept the request
                  using (log.Context(
                     KnownProperty.RoleName, "Twitter Server",
                     KnownProperty.RoleInstance, Guid.NewGuid().ToString()))
                  {
                     using (log.TrackIncomingRequest("getTweets"))
                     {
                        log.Write(LogSeverity.Information, "i've got those tweets!");
                     }
                  }
               }

               //make another request to twitter API
               using (log.TrackOutgoingRequest("Twitter", "getTweets", "getTweets?type=everyoneElses"))
               {

               }

               using (log.TrackOutgoingRequest("Twitter", "setTweets", "setTweets?count=" + DateTime.UtcNow.Millisecond))
               {

               }

            }
         }

         C.ReadKey();
      }
   }
}
