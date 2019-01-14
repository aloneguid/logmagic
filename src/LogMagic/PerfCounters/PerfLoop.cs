using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using LogMagic.PerfCounters.Windows;

namespace LogMagic.PerfCounters
{
   class PerfLoop
   {
      private readonly ILog log;
      private readonly ILogConfiguration _configuration;
      private TimeSpan _samplingInterval = TimeSpan.FromSeconds(10);

      public PerfLoop(ILog log, ILogConfiguration configuration)
      {
         this.log = log ?? throw new ArgumentNullException(nameof(log));
         _configuration = configuration;

         var thread = new Thread(Loop)
         {
            IsBackground = true
         };

         thread.Start();
      }

      private void Loop()
      {
         while(true)
         {
            try
            {
               foreach(IPerformanceCounter ipc in _configuration.PerformanceCounters)
               {
                  float value = ipc.GetValue();
                  log.Metric(ipc.Name, value);
               }
            }
            catch(Exception ex)
            {
               Trace.WriteLine("fatal error: " + ex);
            }

            Thread.Sleep(_samplingInterval);
         }
      }
   }
}
