using LogMagic.Storage.Net;
using Storage.Net.Blob;
using Storage.Net.KeyValue;
using System;

namespace LogMagic
{
   /// <summary>
   /// Extension methods to configure the integration
   /// </summary>
   public static class ConfigurationExtensions
   {
      /// <summary>
      /// Writes logs to a blob
      /// </summary>
      /// <param name="configuration">Configuration reference</param>
      /// <param name="blobStorage">Valid blob storage reference</param>
      /// <param name="documentId">ID of the document to append to</param>
      /// <param name="format">Optional format string</param>
      /// <returns></returns>
      public static ILogConfiguration StorageAppendBlob(this ILogConfiguration configuration,
         IBlobStorage blobStorage,
         string documentId,
         string format = null)
      {
         return configuration.AddWriter(new BlobStorageLogWriter(blobStorage, documentId, format));
      }

      /// <summary>
      /// Initialises logging to key-value storage
      /// </summary>
      public static ILogConfiguration StorageKeyValue(this ILogConfiguration configuration,
         IKeyValueStorage tableStorage)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Initialises logging to work with message publisher
      /// </summary>
      /// <param name="configuration"></param>
      /// <returns></returns>
      public static ILogConfiguration StorageMessagePublisher(this ILogConfiguration configuration)
      {
         throw new NotImplementedException();
      }
   }
}