using Xunit;

namespace LogMagic.Test
{
   public class FiltersTest
   {
      private TestWriter _writer = new TestWriter();
      private ILog log = L.G<FiltersTest>();

      public FiltersTest()
      {
         L.Config.ClearWriters();
         L.Config.ClearFilters();

      }

      [Fact]
      public void SeverityFilter_WithMinSeverity_LogsAllEvents()
      {
         L.Config
            .WriteTo.Custom(_writer)
            .When.SeverityIsAtLeast(LogSeverity.Verbose);


         log.Write(nameof(SeverityFilter_WithMinSeverity_LogsAllEvents));

         Assert.Equal(nameof(SeverityFilter_WithMinSeverity_LogsAllEvents), _writer.Event.Message);
      }

      [Fact]
      public void SeverityFilter_WithInfoSeverity_DoesntLog()
      {
         L.Config
            .WriteTo.Custom(_writer)
            .When.SeverityIsAtLeast(LogSeverity.Information);

         log.Write(nameof(SeverityFilter_WithInfoSeverity_DoesntLog));

         Assert.Null(_writer.Event);
      }
   }
}
