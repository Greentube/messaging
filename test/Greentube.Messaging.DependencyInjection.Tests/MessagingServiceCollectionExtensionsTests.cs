using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Greentube.Serialization;
using Xunit;

namespace Greentube.Messaging.DependencyInjection.Tests
{
    public class MessagingServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddMessaging_NoCallsOnBuilder_ThrowsInvalidOperation()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => services.AddMessaging(builder => { }));
        }

        [Fact]
        public void AddMessaging_RequiresBuilderAction()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => services.AddMessaging(null));
        }

        [Fact]
        public void AddMessaging_TopicSerializerRawPublisherAndRawSubscriber_BuildsMessaging()
        {
            // Arrange
            var s = new ServiceCollection();

            // Act
            s.AddMessaging(builder =>
            {
                builder.AddSerialization(b => b.AddSerializer<ISerializer>(ServiceLifetime.Singleton));
                builder.AddRawMessageHandlerSubscriber<IRawMessageHandlerSubscriber>();
                builder.AddRawMessagePublisher<IRawMessagePublisher>();
                builder.AddTopic<MessagingServiceCollectionExtensionsTests>(
                    nameof(MessagingServiceCollectionExtensionsTests));
            });

            // Assert
            AssertMessagingBuilt(s);
        }

        [Fact]
        public void AddMessaging_TopicSerializerRawPublisherAndRawSubscriber_AddedViaServices_BuildsMessaging()
        {
            // Arrange
            var s = new ServiceCollection();
            s.AddSingleton<ISerializer, ISerializer>();
            s.AddSingleton<IRawMessageHandlerSubscriber, IRawMessageHandlerSubscriber>();
            s.AddSingleton<IRawMessagePublisher, IRawMessagePublisher>();
            s.AddSingleton<IMessageTypeTopicMap>(new MessageTypeTopicMap
            {
                {typeof(MessagingServiceCollectionExtensionsTests),
                    nameof(MessagingServiceCollectionExtensionsTests)}
            });

            // Act
            s.AddMessaging(builder => { });

            // Assert
            AssertMessagingBuilt(s);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void AssertMessagingBuilt(ServiceCollection s)
        {
            // Services required to be provided by the caller of AddMessaging:
            Assert.Equal(1, s.Count(d => d.ServiceType == typeof(ISerializer)));
            Assert.Equal(1, s.Count(d => d.ServiceType == typeof(IRawMessageHandlerSubscriber)));
            Assert.Equal(1, s.Count(d => d.ServiceType == typeof(IRawMessagePublisher)));
            Assert.Equal(1, s.Count(d => d.ServiceType == typeof(IMessageTypeTopicMap)));
        }
    }
}