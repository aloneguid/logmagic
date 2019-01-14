using System;
using System.Diagnostics;

namespace LogMagic.PerfCounters.Windows
{
   /// <summary>
   /// Base class for implementing Windows OS based performance counters
   /// </summary>
   class WindowsPerformanceCounter : IPerformanceCounter
   {
      private readonly PerformanceCounter _performanceCounter;
      private CounterSample _lastSample;

      public WindowsPerformanceCounter(string name,
         string categoryName, string counterName, string instanceName)
      {
         Name = name;

         _performanceCounter = new PerformanceCounter(categoryName, counterName, instanceName);
      }

      public string Name { get; private set; }

      public float GetValue()
      {
         if(_lastSample == null)
         {
            _lastSample = _performanceCounter.NextSample();
            return _performanceCounter.NextValue();
         }

         CounterSample sample = _performanceCounter.NextSample();

         //calculate the difference
         float value = CounterSample.Calculate(_lastSample, sample);
         _lastSample = sample;
         return value;
      }
   }
}