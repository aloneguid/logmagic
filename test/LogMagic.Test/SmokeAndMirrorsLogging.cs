using Xunit;
using System.IO;
using System;
using System.Threading;
using Storage.Net;
using System.Net;
using Config.Net;

namespace LogMagic.Test
{
   public class FilesWriterTest : SmokeAndMirrorsLogging
   {
      public FilesWriterTest() : base("files")
      {

      }
   }

   public class ConsoleWriterTest : SmokeAndMirrorsLogging
   {
      public ConsoleWriterTest() : base("console")
      {

      }
   }

   public class PoshConsoleWriterTest : SmokeAndMirrorsLogging
   {
      public PoshConsoleWriterTest() : base("posh-console")
      {

      }
   }

   public class TraceWriterTest : SmokeAndMirrorsLogging
   {
      public TraceWriterTest() : base("trace")
      {

      }
   }

   public class AppInsightsWriterTest : SmokeAndMirrorsLogging
   {
      public AppInsightsWriterTest() : base("azure-appinsights")
      {

      }
   }


   public abstract class SmokeAndMirrorsLogging : AbstractTestFixture
   {
      protected SmokeAndMirrorsLogging(string receiverName)
      {
         ISettings settings = new ConfigurationBuilder<ISettings>()
            .UseIniFile("c:\\tmp\\integration-tests.ini")
            .UseEnvironmentVariables()
            .Build();

         L.Config.Reset();

         switch (receiverName)
         {
            case "files":
               L.Config.AddFile(Path.Combine(TestDir.FullName, "subdir", "testlog.txt"));
               break;
            case "console":
               L.Config.AddConsole();
               break;
            case "posh-console":
               L.Config.AddPoshConsole();
               break;
            case "trace":
               L.Config.AddTrace();
               break;
            case "azure-appinsights":
               L.Config.AddAzureApplicationInsights(settings.AppInsightsKey);
               break;
         }   
      }

      [Fact]
      public void Smoke_SomethingSimple_DoestCrash()
      {
         ILog log = L.G<SmokeAndMirrorsLogging>();
         ILog _log = L.G<SmokeAndMirrorsLogging>();

         //L.Config.FilterBy.

         log.Write("hello!");
         log.Write("exception is here!", new Exception("test exception"));

         for(int i = 0; i < 100; i++)
         {
            log.Write("test {i}", i);
         }
      }
   }
}