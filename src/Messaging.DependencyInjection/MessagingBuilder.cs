using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Serialization;

namespace Messaging.DependencyInjection
{
    public class MessagingBuilder
    {
        public IServiceCollection Services { get; }
        private readonly MessageTypeTopicMap _messageTypeTopic = new MessageTypeTopicMap();
        private readonly DiscoverySettings _discoverySettings = new DiscoverySettings();

        internal MessagingBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public MessagingBuilder ConfigureOptions(Action<MessagingOptions> options)
        {
            Services.Configure(options);
            return this;
        }

        public MessagingBuilder AddSerializer<TSerializer>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TSerializer : ISerializer
        {
            Services.Add(ServiceDescriptor.Describe(typeof(ISerializer), typeof(TSerializer), lifetime));
            return this;
        }

        public MessagingBuilder AddRawMessagePublisher<TRawMessagePublisher>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TRawMessagePublisher : IRawMessagePublisher
        {
            Services.Add(ServiceDescriptor.Describe(typeof(IRawMessagePublisher), typeof(TRawMessagePublisher), lifetime));
            return this;
        }

        public MessagingBuilder AddRawMessageHandlerSubscriber<TRawMessageHandlerSubscriber>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TRawMessageHandlerSubscriber : IRawMessageHandlerSubscriber
        {
            Services.Add(ServiceDescriptor.Describe(typeof(IRawMessageHandlerSubscriber), typeof(TRawMessageHandlerSubscriber), lifetime));
            return this;
        }

        public MessagingBuilder AddTopic<TMessage>(string topic)
        {
            _messageTypeTopic.Add(typeof(TMessage), topic);
            return this;
        }

        public MessagingBuilder AddHandlerDiscovery(Action<DiscoverySettings> discoverySettings)
        {
            discoverySettings(_discoverySettings);
            return this;
        }

        private void AddHandlerDiscovery()
        {
            // Look also on assemblies where there are Message types:
            _discoverySettings.MessageHandlerAssemblies.AddRange(_messageTypeTopic.Select(t => t.Key.Assembly).Distinct());

            Services.Scan(s =>
                s.FromAssemblies(_discoverySettings.MessageHandlerAssemblies)
                    .AddClasses(f => f.AssignableTo(typeof(IMessageHandler<>)), !_discoverySettings.IncludeNonPublic)
                    .UsingRegistrationStrategy(_discoverySettings.RegistrationStrategy)
                    .AsImplementedInterfaces()
                    .WithLifetime(_discoverySettings.DiscoveredHandlersLifetime));

            Services.TryAddSingleton(_discoverySettings.MessageHandlerAssemblies);
        }

        public void Build()
        {
            var support = VerifyMessagingSupport();

            // Configuration
            Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MessagingOptions>, DefaultMessagingOptionsSetup>());
            Services.AddSingleton(c => c.GetRequiredService<IOptions<MessagingOptions>>().Value);

            // Map: used by both Publishing and subscribing
            Services.TryAddSingleton<IMessageTypeTopicMap>(_messageTypeTopic);

            if (support.supportPublishing)
            {
                Services.TryAddSingleton<IMessagePublisher, SerializedMessagePublisher>();
            }

            if (support.supportHandling)
            {
                Services.TryAddSingleton<IRawMessageHandler, DispatchingRawMessageHandler>();
                Services.TryAddSingleton<IMessageHandlerInfoProvider, MessageHandlerInfoProvider>();
                Services.TryAddSingleton<IMessageHandlerInvoker>(c =>
                    new MessageHandlerInvoker(
                        c.GetService<IMessageHandlerInfoProvider>(),
                        c.GetService));

                AddHandlerDiscovery();
            }
        }

        private (bool supportPublishing, bool supportHandling) VerifyMessagingSupport()
        {
            var serializers = Services.Count(s => s.ServiceType == typeof(ISerializer));
            if (serializers == 0) throw new InvalidOperationException($"No serializer has been configured. Call builder.{nameof(AddSerializer)}");
            if (serializers > 1) throw new InvalidOperationException("More than one serializer has been configured. Please define a single one.");

            var rawPublishers = Services.Count(s => s.ServiceType == typeof(IRawMessagePublisher));
            if (rawPublishers > 1) throw new InvalidOperationException("More than one raw publisher has been configured. Please define a single one.");

            var rawHandlerSubscribers = Services.Count(s => s.ServiceType == typeof(IRawMessageHandlerSubscriber));
            if (rawHandlerSubscribers > 1) throw new InvalidOperationException("More than one raw handler subscriber has been configured. Please define a single one.");

            (bool supportPublishing, bool supportHandling) support = (rawPublishers == 1, rawHandlerSubscribers == 1);

            if (!support.supportHandling && !support.supportPublishing)
                throw new InvalidOperationException("No raw publisher or raw handler subscriber have been configured. " +
                                                    "It won't be possible to either Publish nor Handle messages. " +
                                                    $"To Publish messages, call builder.{nameof(AddRawMessagePublisher)} " +
                                                    $"To Handle messages, call builder.{nameof(AddRawMessageHandlerSubscriber)}.");

            // No calls to AddTopic and no IMessageTypeTopicMap registered yet, we fail
            if (!_messageTypeTopic.Any() &&
                Services.All(s => s.ServiceType != typeof(IMessageTypeTopicMap)))
            {
                throw new InvalidOperationException($"Can't build messaging without any topics. Consider calling '{nameof(AddTopic)}' on the builder " +
                                                    $"or register your own: {nameof(IMessageTypeTopicMap)} before adding messaging.");
            }

            return support;
        }
    }
}