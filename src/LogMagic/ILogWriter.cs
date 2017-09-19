﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogMagic
{
   /// <summary>
   /// Common interface for a log receiver
   /// </summary>
   public interface ILogWriter : IDisposable
   {
      /// <summary>
      /// Writes a sequence of log events to the target.
      /// </summary>
      void Write(IEnumerable<LogEvent> events);
   }
}
