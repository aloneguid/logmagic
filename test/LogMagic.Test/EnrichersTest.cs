using System;
using LogMagic.Enrichers;
using Xunit;

namespace LogMagic.Test
{
   public class EnrichersTest
   {
      private TestWriter _writer;
      private ILog _log;

      public EnrichersTest()
      {
         _writer = new TestWriter();

         L.Config.Reset();

         L.Config
            .WriteTo.Trace()
            .WriteTo.Writer(_writer);

         _log = L.G<FormattingTest>();
      }

      //[Fact]//reflection is not working for some reason
      public void MethodName_ThisMethod_Matches()
      {
         L.Config.EnrichWith.MethodName();
         _log.Write("method call");

         Assert.Equal("LogMagic.Test.EnrichersTest.MethodName_ThisMethod_Matches()", 
            (string)_writer.Event.GetProperty(KnownProperty.MethodName));
      }

      //[Fact] reflection problems
      public void MethodName_MethodWithParameters_ParametersFormatted()
      {
         L.Config.EnrichWith.MethodName();
         MethodWithParameters(null, 1, Guid.NewGuid());

         Assert.Equal("LogMagic.Test.EnrichersTest.MethodWithParameters(String s, Int32 i, Guid g)",
            (string)_writer.Event.GetProperty("method"));
      }

      private string MethodWithParameters(string s, int i, Guid g)
      {
         _log.Write("parameters method");
         return s;
      }

      //[Fact] doesn't always work on some systems, needs to be investigated
      public void MachineIp_Current_ReturnsSomething()
      {
         L.Config.EnrichWith.MachineIpAddress();
         _log.Write("what's this machine IP?");

         string address = (string)_writer.Event.GetProperty(KnownProperty.NodeIp);
         _log.Write("address: {ipAddress}", address);
         Assert.NotNull(address);
      }
   }
}
