using LogMagic.Enrichers;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.V2.Runtime;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogMagic.Microsoft.ServiceFabric.Remoting
{
   class CorrelatingRemotingMessageHandler : IServiceRemotingMessageHandler
   {
      private static readonly Encoding Enc = Encoding.UTF8;
      private readonly IServiceRemotingMessageHandler _innerHandler;
      private readonly ILog _log;
      private readonly Action<CallSummary> _raiseSummary;
      private static FieldInfo getHeadersField;
      private static readonly ILog log = L.G(typeof(CorrelatingRemotingMessageHandler));

      public CorrelatingRemotingMessageHandler(ILog log, ServiceContext serviceContext, IService serviceImplementation, Action<CallSummary> raiseSummary)
      {
         _innerHandler = new ServiceRemotingMessageDispatcher(serviceContext, serviceImplementation);

         MethodResolver.AddMethodsForProxyOrService(serviceImplementation.GetType().GetInterfaces(), typeof(IService));
         _log = log;
         _raiseSummary = raiseSummary;
      }

      public CorrelatingRemotingMessageHandler(ILog log, ActorService actorService)
      {
         _innerHandler = new ActorServiceRemotingDispatcher(actorService, null);

         MethodResolver.AddMethodsForProxyOrService(actorService.GetType().GetInterfaces(), typeof(IService));
         MethodResolver.AddMethodsForProxyOrService(actorService.ActorTypeInformation.InterfaceTypes, typeof(IActor));

         _log = log;
         _raiseSummary = null;
      }

      public IServiceRemotingMessageBodyFactory GetRemotingMessageBodyFactory()
      {
         return _innerHandler.GetRemotingMessageBodyFactory();
      }

      public void HandleOneWayMessage(IServiceRemotingRequestMessage requestMessage)
      {
         _innerHandler.HandleOneWayMessage(requestMessage);
      }

      public async Task<IServiceRemotingResponseMessage> HandleRequestResponseAsync(
         IServiceRemotingRequestContext requestContext,
         IServiceRemotingRequestMessage requestMessage)
      {
         Dictionary<string, string> context = ExtractContextProperties(requestMessage);
         string methodName = MethodResolver.GetMethodName(requestMessage);

         Exception gex = null;
         using (log.Context(context))
         {
            using (var time = new TimeMeasure())
            {
               try
               {
                  return await _innerHandler.HandleRequestResponseAsync(requestContext, requestMessage);
               }
               catch (Exception ex)
               {
                  gex = ex;
                  throw;
               }
               finally
               {
                  if (_raiseSummary != null)
                  {
                     var summary = new CallSummary(methodName, gex, time.ElapsedTicks);
                     _raiseSummary(summary);
                  }

                  //todo: slightly broken
                  //_log.TrackIncomingRequest(methodName, time.ElapsedTicks, gex,
                  //   context.ToDictionary(k => k.Key, v => (object)(v.Value)));
               }
            }
         }
      }

      private Dictionary<string, string> ExtractContextProperties(IServiceRemotingRequestMessage message)
      {
         IServiceRemotingRequestMessageHeader headers = message.GetHeader();

         //DANGER!!! this is using reflection to get to internal dictionary of headers collection.
         if (getHeadersField == null)
         {
            getHeadersField = headers.GetType()
               .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
               .First(f => f.Name == "headers");
         }
         var headersCollection = getHeadersField.GetValue(headers) as Dictionary<string, byte[]>;

         if (headersCollection == null) return null;

         var result = headersCollection.ToDictionary(
            e => e.Key,
            e => e.Value == null ? null : Enc.GetString(e.Value));

         // ActivityId becomes parent here
         if(result.TryGetValue(KnownProperty.ActivityId, out string activityId))
         {
            result[KnownProperty.ParentActivityId] = activityId;
            result.Remove(KnownProperty.ActivityId);
         }

         return result;
      }
   }
}
