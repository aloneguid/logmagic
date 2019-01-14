using System;
using System.Collections.Generic;
using System.Text;

namespace LogMagic.PerfCounters
{
   class ManagedMemoryPerformanceCounter : IPerformanceCounter
   {
      public string Name => "Managed Memory";

      public float GetValue()
      {
         return Environment.WorkingSet;
      }
   }
}