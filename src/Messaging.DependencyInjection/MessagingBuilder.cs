using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            Services = services;
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
            var serializers = Services.Count(s => s.ServiceType == typeof(ISerializer));
            if (serializers == 0)
            {
                throw new InvalidOperationException("No serializer has been configured.");
            }
            if (serializers > 1)
            {
                throw new InvalidOperationException(
                    "More than one serializer has been configured. Please define a single serializer.");
            }

            // No calls to AddTopic and no IMessageTypeTopicMap registered yet, we fail
            if (!_messageTypeTopic.Any() &&
                Services.All(s => s.ServiceType != typeof(IMessageTypeTopicMap)))
            {
                throw new InvalidOperationException($"Can't build messaging without any topics. Consider calling '{nameof(AddTopic)}' on the builder " +
                                                    $"or register your own: {nameof(IMessageTypeTopicMap)} before adding messaging.");
            }

            Services.TryAddSingleton<IMessageTypeTopicMap>(_messageTypeTopic);

            _discoverySettings.MessageHandlerAssemblies.AddRange(_messageTypeTopic.Select(t => t.Key.Assembly).Distinct());
            AddHandlerDiscovery();
        }
    }
}