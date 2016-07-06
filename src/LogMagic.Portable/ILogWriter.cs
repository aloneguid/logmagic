﻿using System;
using System.Collections.Generic;

namespace LogMagic
{
   /// <summary>
   /// Common interface for a log receiver
   /// </summary>
   public interface ILogWriter : IDisposable
   {
      void Write(IEnumerable<LogEvent> events);
   }
}
