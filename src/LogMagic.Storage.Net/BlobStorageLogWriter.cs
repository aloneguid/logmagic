using LogMagic.Tokenisation;
using NetBox.Extensions;
using Storage.Net.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LogMagic.Storage.Net
{
   class BlobStorageLogWriter : ILogWriter
   {
      private readonly IBlobStorage _blobStorage;
      private readonly FormattedString _format;
      private readonly string _documentId;

      public BlobStorageLogWriter(IBlobStorage blobStorage, string documentId, string format)
      {
         _blobStorage = blobStorage ?? throw new ArgumentNullException(nameof(blobStorage));
         _format = format == null ? null : FormattedString.Parse(format, null);
         _documentId = documentId ?? throw new ArgumentNullException(nameof(documentId));
      }

      public void Write(IEnumerable<LogEvent> events)
      {
         if (events == null) return;

         var sb = new StringBuilder();
         foreach (LogEvent e in events)
         {
            sb.AppendLine(TextFormatter.Format(e, _format));
         }

         Task.Run(() =>
         {
            using (MemoryStream ms = sb.ToString().ToMemoryStream())
            {
               _blobStorage.WriteAsync(_documentId, ms, true);
            }
         });
      }

      public void Dispose()
      {
         _blobStorage.Dispose();
      }
   }
}
