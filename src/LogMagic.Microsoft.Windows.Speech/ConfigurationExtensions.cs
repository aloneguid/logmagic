using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogMagic.Microsoft.Windows.Speech;

namespace LogMagic
{
   public static class ConfigurationExtensions
   {
      public static ILogConfiguration WindowsSpeech(this IWriterConfiguration configuration,
         string whatToSayOnError, bool speakErrorMessage)
      {
         return configuration.Writer(new WindowsSpeechLogWriter(whatToSayOnError, speakErrorMessage));
      }

   }
}
