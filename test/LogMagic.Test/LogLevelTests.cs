using LogMagic;
using LogMagic.Levels;
using Xunit;

namespace LogMagic.Test
{
    public class LogLevelTests
    {
      [Fact]
      public void SetLogLevel_CheckOutput_Verbose()
      {
         L.Config.LogLevel = LogSeverity.Verbose;
         Assert.True(LogLevelChecks.Check(LogSeverity.Verbose));
         Assert.True(LogLevelChecks.Check(LogSeverity.Information));
         Assert.True(LogLevelChecks.Check(LogSeverity.Warning));
         Assert.True(LogLevelChecks.Check(LogSeverity.Error));
         Assert.True(LogLevelChecks.Check(LogSeverity.Critical));
      }

      [Fact]
      public void SetLogLevel_CheckOutput_Information()
      {
         L.Config.LogLevel = LogSeverity.Information;
         Assert.False(LogLevelChecks.Check(LogSeverity.Verbose));
         Assert.True(LogLevelChecks.Check(LogSeverity.Information));
         Assert.True(LogLevelChecks.Check(LogSeverity.Warning));
         Assert.True(LogLevelChecks.Check(LogSeverity.Error));
         Assert.True(LogLevelChecks.Check(LogSeverity.Critical));
      }

      [Fact]
      public void SetLogLevel_CheckOutput_Warning()
      {
         L.Config.LogLevel = LogSeverity.Warning;
         Assert.False(LogLevelChecks.Check(LogSeverity.Verbose));
         Assert.False(LogLevelChecks.Check(LogSeverity.Information));
         Assert.True(LogLevelChecks.Check(LogSeverity.Warning));
         Assert.True(LogLevelChecks.Check(LogSeverity.Error));
         Assert.True(LogLevelChecks.Check(LogSeverity.Critical));
      }

      [Fact]
      public void SetLogLevel_CheckOutput_Error()
      {
         L.Config.LogLevel = LogSeverity.Error;
         Assert.False(LogLevelChecks.Check(LogSeverity.Verbose));
         Assert.False(LogLevelChecks.Check(LogSeverity.Information));
         Assert.False(LogLevelChecks.Check(LogSeverity.Warning));
         Assert.True(LogLevelChecks.Check(LogSeverity.Error));
         Assert.True(LogLevelChecks.Check(LogSeverity.Critical));
      }

      [Fact]
      public void SetLogLevel_CheckOutput_Critical()
      {
         L.Config.LogLevel = LogSeverity.Critical;
         Assert.False(LogLevelChecks.Check(LogSeverity.Verbose));
         Assert.False(LogLevelChecks.Check(LogSeverity.Information));
         Assert.False(LogLevelChecks.Check(LogSeverity.Warning));
         Assert.False(LogLevelChecks.Check(LogSeverity.Error));
         Assert.True(LogLevelChecks.Check(LogSeverity.Critical));
      }
   }
}
