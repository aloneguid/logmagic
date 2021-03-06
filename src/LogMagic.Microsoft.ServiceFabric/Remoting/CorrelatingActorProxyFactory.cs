﻿using System;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Remoting;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V2.Client;

namespace LogMagic.Microsoft.ServiceFabric.Remoting
{
   class CorrelatingActorProxyFactory<TActorInterface> : IActorProxyFactory where TActorInterface : IActor
   {
      private readonly ActorProxyFactory _actorProxyFactory;
      private readonly string _defaultListenerName;
      private readonly ActorRemotingProviderAttribute _providerAttribute;

      public CorrelatingActorProxyFactory(
         Func<IServiceRemotingCallbackMessageHandler, IServiceRemotingClientFactory> createServiceRemotingClientFactory = null,
         OperationRetrySettings retrySettings = null,
         Action<CallSummary> raiseSummary = null,
         string remoteServiceName = null)
      {
         _defaultListenerName = ActorHelper.GetDefaultListenerName<TActorInterface>(out _providerAttribute);

         if(_providerAttribute != null)
         {
            createServiceRemotingClientFactory = _providerAttribute.CreateServiceRemotingClientFactory;
         }

         //_actorProxyFactory = new ActorProxyFactory();

         _actorProxyFactory = new ActorProxyFactory(
            (callbackClient) =>
            {
               if (createServiceRemotingClientFactory == null)
               {
                  return new CorrelatingFabricTransportServiceRemotingClientFactory<TActorInterface>(inner: null, raiseSummary: raiseSummary, remoteServiceName: remoteServiceName);
               }

               IServiceRemotingClientFactory innerClientFactory = createServiceRemotingClientFactory(callbackClient);

               if (innerClientFactory is CorrelatingFabricTransportServiceRemotingClientFactory<TActorInterface>) return innerClientFactory;

               return new CorrelatingFabricTransportServiceRemotingClientFactory<TActorInterface>(inner: innerClientFactory, raiseSummary: raiseSummary, remoteServiceName: remoteServiceName);

            },
            retrySettings);
      }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
      public TActorInterface CreateActorProxy<TActorInterface>(ActorId actorId, string applicationName = null, string serviceName = null, string listenerName = null) where TActorInterface : IActor
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
      {
         TActorInterface proxy = _actorProxyFactory.CreateActorProxy<TActorInterface>(actorId, applicationName, serviceName, GetListenerName(listenerName));

         MethodResolver.AddMethodsForProxyOrService(proxy.GetType().GetInterfaces(), typeof(IActor));

         return proxy;
      }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
      public TActorInterface CreateActorProxy<TActorInterface>(Uri serviceUri, ActorId actorId, string listenerName = null) where TActorInterface : IActor
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
      {
         TActorInterface proxy = _actorProxyFactory.CreateActorProxy<TActorInterface>(serviceUri, actorId, GetListenerName(listenerName));

         MethodResolver.AddMethodsForProxyOrService(proxy.GetType().GetInterfaces(), typeof(IActor));

         return proxy;
      }

      public TServiceInterface CreateActorServiceProxy<TServiceInterface>(Uri serviceUri, ActorId actorId, string listenerName = null) where TServiceInterface : IService
      {
         TServiceInterface proxy = _actorProxyFactory.CreateActorServiceProxy<TServiceInterface>(serviceUri, actorId, GetListenerName(listenerName));

         MethodResolver.AddMethodsForProxyOrService(proxy.GetType().GetInterfaces(), typeof(IService));

         return proxy;
      }

      public TServiceInterface CreateActorServiceProxy<TServiceInterface>(Uri serviceUri, long partitionKey, string listenerName = null) where TServiceInterface : IService
      {
         TServiceInterface proxy = _actorProxyFactory.CreateActorServiceProxy<TServiceInterface>(serviceUri, partitionKey, GetListenerName(listenerName));

         MethodResolver.AddMethodsForProxyOrService(proxy.GetType().GetInterfaces(), typeof(IService));

         return proxy;
      }

      private string GetListenerName(string listenerName)
      {
         return _defaultListenerName ?? listenerName;
      }

   }
}