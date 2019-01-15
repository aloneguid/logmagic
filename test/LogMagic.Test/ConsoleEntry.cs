using System;
using LogMagic;

namespace LogMagicExample
{
   public class Program
   {
      private readonly ILog _log = L.G<Program>();

      public static void Main(string[] args)
      {
         L.Config
            .AddPoshConsole("{time:H:mm:ss,fff}|{level}|{threadId,4}|{source}|{message}{error}")
            .AddConsole()
            .EnrichWith.ThreadId();

         new Program().Run();

         Console.ReadLine();
      }

      private void Run()
      {
         _log.Write(LogSeverity.Information, "hello, LogMagic!");

         _log.Write(LogSeverity.Information, "we are going to divide by zero!");

         int a = 10, b = 0;

         try
         {
            _log.Write(LogSeverity.Information, "dividing {a} by {b}", a, b);
            Console.WriteLine(a / b);
         }
         catch(Exception ex)
         {
            _log.Write(LogSeverity.Information, "unexpected error", ex);
         }

         _log.Write(LogSeverity.Information, "attempting to divide by zero");
      }

   }
}
