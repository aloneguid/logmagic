﻿using Xunit;

namespace LogMagic.Test
{
   public class FiltersTest
   {
      private TestWriter _writer = new TestWriter();
      private ILog log = L.G<FiltersTest>();

      public FiltersTest()
      {
         L.Config.Reset();
      }

      [Fact]
      public void SeverityFilter_WithMinSeverity_LogsAllEvents()
      {
         L.Config
            .AddWriter(_writer)
            .FilterBy.MinLogSeverity(LogSeverity.Verbose);


         log.Info(nameof(SeverityFilter_WithMinSeverity_LogsAllEvents));

         Assert.Equal(nameof(SeverityFilter_WithMinSeverity_LogsAllEvents), _writer.Event.Message);
      }

      [Fact]
      public void SeverityFilter_WithInfoSeverity_DoesntLog()
      {
         L.Config
            .AddWriter(_writer)
            .FilterBy.MinLogSeverity(LogSeverity.Information);

         log.Info(nameof(SeverityFilter_WithInfoSeverity_DoesntLog));

         Assert.Null(_writer.Event);
      }
   }
}
