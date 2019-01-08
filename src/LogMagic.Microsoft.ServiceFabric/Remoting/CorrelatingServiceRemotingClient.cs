using LogMagic.Enrichers;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;
using NetBox.Extensions;
using System;
using System.Fabric;
using System.Threading.Tasks;

namespace LogMagic.Microsoft.ServiceFabric.Remoting
{
   class CorrelatingServiceRemotingClient : IServiceRemotingClient
   {
      private readonly ILog _log;
      private readonly IServiceRemotingClient _inner;
      private readonly Action<CallSummary> _raiseSummary;
      private readonly string _remoteServiceName;
      private readonly RequestEnricher _enricher;

      public CorrelatingServiceRemotingClient(ILog log, IServiceRemotingClient inner, Action<CallSummary> raiseSummary, string remoteServiceName)
      {
         _log = log;
         _inner = inner;
         _raiseSummary = raiseSummary;
         _remoteServiceName = remoteServiceName;
         _enricher = new RequestEnricher();
      }

      public IServiceRemotingClient Inner => _inner;

      public ResolvedServicePartition ResolvedServicePartition
      {
         get => _inner.ResolvedServicePartition;
         set => _inner.ResolvedServicePartition = value;
      }

      public string ListenerName
      {
         get => _inner.ListenerName;
         set => _inner.ListenerName = value;
      }

      public ResolvedServiceEndpoint Endpoint
      {
         get => _inner.Endpoint;
         set => _inner.Endpoint = value;
      }

      public async Task<IServiceRemotingResponseMessage> RequestResponseAsync(IServiceRemotingRequestMessage requestMessage)
      {
         string dependencyId = Guid.NewGuid().ToShortest();
         string methodName = MethodResolver.GetMethodName(requestMessage);

         using (var time = new TimeMeasure())
         {
            //using (_log.Context(KnownProperty.ActivityId, dependencyId))   //dependency ID travels to the server as parent Id
            //{
               Exception gex = null;
               _enricher.Enrich(requestMessage);

               try
               {
                  IServiceRemotingResponseMessage response = await _inner.RequestResponseAsync(requestMessage);

                  return response;
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

                  //ActivityId is overriden by context
                  //_log.TrackOutgoingRequest(dependencyId, _remoteServiceName, methodName, time.ElapsedTicks, gex);
               }
            //}
         }
      }

      public void SendOneWay(IServiceRemotingRequestMessage requestMessage)
      {
         _enricher.Enrich(requestMessage);

         _inner.SendOneWay(requestMessage);
      }
   }
}
