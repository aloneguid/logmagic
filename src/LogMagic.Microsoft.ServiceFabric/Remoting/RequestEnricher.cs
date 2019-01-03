using LogMagic;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using System.Collections.Generic;
using System.Text;

namespace LogMagic.Microsoft.ServiceFabric.Remoting
{
   class RequestEnricher
   {
      private static readonly Encoding Enc = Encoding.UTF8;

      private static readonly ILog log = L.G(typeof(RequestEnricher));

      public void Enrich(IServiceRemotingRequestMessage message)
      {
         IDictionary<string, object> context = log.GetContextValues();
         if (context.Count == 0) return;

         IServiceRemotingRequestMessageHeader headers = message.GetHeader();

         foreach(KeyValuePair<string, object> cv in context)
         {
            AddHeader(headers, cv);
         }
      }

      private static void AddHeader(IServiceRemotingRequestMessageHeader headers, KeyValuePair<string, object> header)
      {
         //don't add a header if it already exists
         //todo: figure out why it already exists
         if (headers.TryGetHeaderValue(header.Key, out byte[] headerValue)) return;

         byte[] value = header.Value == null ? null : Enc.GetBytes(header.Value?.ToString());

         headers.AddHeader(header.Key, value);
      }
   }
}
