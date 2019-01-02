using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LogMagic.Writers
{
   //see for more: http://docs.getseq.net/docs/posting-raw-events
   class SeqWriter : ILogWriter
   {
      private readonly HttpClient _client;
      private const string PostUrl = "/api/events/raw";
      private const string ContentType = "application/json";

      public SeqWriter(Uri serverAddress, string apiKey)
      {
         _client = new HttpClient();
         if (apiKey != null) _client.DefaultRequestHeaders.Add("X-Seq-ApiKey", apiKey);
         _client.BaseAddress = serverAddress;
         _client.DefaultRequestHeaders.Accept.Clear();
         _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
      }

      public void Write(IEnumerable<LogEvent> events)
      {
         _client.PostAsync(PostUrl, new StringContent(ToJson(events), Encoding.UTF8, ContentType)).Wait();
      }

      public async Task WriteAsync(IEnumerable<LogEvent> events)
      {
         await _client.PostAsync(PostUrl, new StringContent(ToJson(events), Encoding.UTF8, ContentType));
      }

      private static string ToJson(IEnumerable<LogEvent> events)
      {
         var data = new RawEvents
         {
            Events = events.Select(e => RawEvent.FromLogEvent(e)).ToArray()
         };

         throw new NotImplementedException();
         //string json = data.ToJsonString();
         //return json;
      }

      public void Dispose()
      {
         _client.Dispose();
      }

      #region [ Model ]

      class RawEvents
      {
         public RawEvent[] Events { get; set; }
      }

      class RawEvent
      {
         public DateTimeOffset Timestamp { get; set; }

         // Uses the Serilog level names
         public string Level { get; set; }

         public string MessageTemplate { get; set; }

         public Dictionary<string, object> Properties { get; set; }

         public string Exception { get; set; }

         public static RawEvent FromLogEvent(LogEvent e)
         {
            var re = new RawEvent
            {
               Timestamp = new DateTimeOffset(e.EventTime),
               Level = "info",
               MessageTemplate = e.Message,
               Exception = e.GetProperty(KnownProperty.Error)?.ToString(),
            };

            re.Properties = new Dictionary<string, object>(e.Properties);
            re.Properties.Add("source", e.SourceName);

            return re;
         }
      }

      #endregion
   }
}