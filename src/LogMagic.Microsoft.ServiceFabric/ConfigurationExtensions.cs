using LogMagic.Microsoft.ServiceFabric.Enrichers;
using System.Fabric;
using LogMagic.Microsoft.ServiceFabric.Writers;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using LogMagic.Microsoft.ServiceFabric.Remoting;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;

namespace LogMagic
{
   public static class ConfigurationExtensions
   {
      /// <summary>
      /// Integrates with Service Fabric Health Reports
      /// </summary>
      /// <param name="configuration"></param>
      /// <param name="context"></param>
      /// <returns></returns>
      public static ILogConfiguration AzureServiceFabricHealthReport(this ILogConfiguration configuration, ServiceContext context)
      {
         return configuration.AddWriter(new HealthReportWriter(context));
      }

      /// <summary>
      /// Enriches logging with Service Fabric specific properties
      /// </summary>
      /// <param name="configuration"></param>
      /// <param name="context">Stateful or stateless service context</param>
      /// <returns></returns>
      public static ILogConfiguration AzureServiceFabricContext(this IEnricherConfiguration configuration, ServiceContext context)
      {
         return configuration.Enricher(new ServiceFabricEnricher(context));
      }

      public static ServiceInstanceListener CreateCorrelatingServiceInstanceListener<TServiceInterface>(this StatelessService service,
         IService serviceImplementation,
         string listenerName = "",
         Action<CallSummary> raiseSummary = null)
      {
         var handler = new CorrelatingRemotingMessageHandler(L.G<TServiceInterface>(), service.Context, serviceImplementation, raiseSummary);

         var listener = new ServiceInstanceListener(c => new FabricTransportServiceRemotingListener(c, handler), listenerName);

         return listener;
      }

      public static ServiceReplicaListener CreateCorrelatingReplicaListener<TServiceInterface>(this StatefulService service,
         IService serviceImplementation,
         string listenerName = "",
         bool listenOnSecondary = false,
         Action<CallSummary> raiseSummary = null)
      {
         var handler = new CorrelatingRemotingMessageHandler(L.G<TServiceInterface>(), service.Context, serviceImplementation, raiseSummary);

         var listener = new ServiceReplicaListener(c => new FabricTransportServiceRemotingListener(c, handler), listenerName);

         return listener;
      }
   }
}
