using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Serialization;
using Xunit;

namespace Messaging.DependencyInjection.Tests
{
    public class MessagingBuilderTests
    {
        class Fixture
        {
            public ServiceCollection ServiceCollection { get; } = new ServiceCollection();
            public MessagingBuilder GetBuilder() => new MessagingBuilder(ServiceCollection);
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void Build_AddHandlerDiscovery_MessageHandlerAssembliesContainsEntryAssembly()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            ValidBuilderSetup(builder);

            var assemblyScanHasEntryAssembly = false;
            builder.AddHandlerDiscovery(h =>
                assemblyScanHasEntryAssembly = h.MessageHandlerAssemblies.Any(a => a == Assembly.GetEntryAssembly()));

            // Act
            builder.Build();

            // Assert
            Assert.True(assemblyScanHasEntryAssembly);
        }

        private class TestMessageHandler : IMessageHandler<MessagingBuilderTests>
        {
            public Task Handle(MessagingBuilderTests message, CancellationToken token) => Task.CompletedTask;
        }

        [Fact]
        public void Build_FindsPrivateHandler()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            ValidBuilderSetup(builder);

            // Act
            builder.AddHandlerDiscovery(h => h.IncludeNonPublic = true);

            // Assert
            builder.Build();

            // Assert
            var handler = builder.Services.FirstOrDefault(
                s => s.ServiceType == typeof(IMessageHandler<MessagingBuilderTests>));

            Assert.NotNull(handler);
            Assert.Equal(typeof(TestMessageHandler), handler.ImplementationType);
        }

        [Fact]
        public void Build_ReplacesHandler()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            ValidBuilderSetup(builder);

            var expectedService = typeof(IMessageHandler<MessagingBuilderTests>);
            var originalLifetime = ServiceLifetime.Singleton;

            // Registration expected to be overwriten
            builder.Services.Add(ServiceDescriptor.Describe(
                expectedService,
                s => null,
                originalLifetime));

            var expectedLifetime = ServiceLifetime.Scoped;

            // Act
            builder.AddHandlerDiscovery(h =>
            {
                h.IncludeNonPublic = true;
                h.DiscoveredHandlersLifetime = expectedLifetime;
                // Configuration to replace any matching service registrations
                h.RegistrationStrategy = RegistrationStrategy.Replace(ReplacementBehavior.ServiceType);
            });

            // Assert
            builder.Build();

            // Assert
            var actualDescriptor = _fixture.ServiceCollection.SingleOrDefault(s => s.ServiceType == expectedService);

            Assert.NotNull(actualDescriptor);
            Assert.NotEqual(originalLifetime, actualDescriptor.Lifetime);
            Assert.Equal(typeof(TestMessageHandler), actualDescriptor.ImplementationType);
        }

        [Fact]
        public void Build_ExternalRequiredServicesRegistered_RegistersAllRequiredServices()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            var s = _fixture.ServiceCollection;
            s.AddSingleton<ISerializer, ISerializer>();
            s.AddSingleton<IRawMessageHandlerSubscriber, IRawMessageHandlerSubscriber>();
            s.AddSingleton<IRawMessagePublisher, IRawMessagePublisher>();
            s.AddSingleton<IMessageTypeTopicMap>(new MessageTypeTopicMap
            {
                {
                    typeof(MessagingServiceCollectionExtensionsTests),
                    nameof(MessagingServiceCollectionExtensionsTests)
                }
            });

            // Act
            builder.Build();

            // Assert
            AssertRequiredServices(s, true, true);
        }

        [Fact]
        public void Build_NoRawSubscriber_NoSubscriptionRelatedServices()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            var s = _fixture.ServiceCollection;
            s.AddSingleton<ISerializer, ISerializer>();
            s.AddSingleton<IRawMessagePublisher, IRawMessagePublisher>();
            s.AddSingleton<IMessageTypeTopicMap>(new MessageTypeTopicMap
            {
                {
                    typeof(MessagingServiceCollectionExtensionsTests),
                    nameof(MessagingServiceCollectionExtensionsTests)
                }
            });

            // Act
            builder.Build();

            // Assert
            AssertRequiredServices(s, true, false);
        }

        [Fact]
        public void Build_NoRawPublisher_NoPublisherRelatedServices()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            var s = _fixture.ServiceCollection;
            s.AddSingleton<ISerializer, ISerializer>();
            s.AddSingleton<IRawMessageHandlerSubscriber, IRawMessageHandlerSubscriber>();
            s.AddSingleton<IMessageTypeTopicMap>(new MessageTypeTopicMap
            {
                {
                    typeof(MessagingServiceCollectionExtensionsTests),
                    nameof(MessagingServiceCollectionExtensionsTests)
                }
            });

            // Act
            builder.Build();

            // Assert
            AssertRequiredServices(s, false, true);
        }

        [Fact]
        public void Build_NoCallsOnBuilder_ThrowsInvalidOperation()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Build_NeitherPublisherNorSubscriber_ThrowsInvalidOperation()
        {
            var builder = _fixture.GetBuilder();
            builder.AddSerializer<ISerializer>()
                .AddTopic<MessagingBuilderTests>(nameof(MessagingBuilderTests));

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Build_NoTopicDefined_ThrowsInvalidOperation()
        {
            var builder = _fixture.GetBuilder();

            builder.AddSerializer<ISerializer>()
                .AddRawMessageHandlerSubscriber<IRawMessageHandlerSubscriber>()
                .AddRawMessagePublisher<IRawMessagePublisher>();

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Build_ValidBuilderSetupHelper_DoesntThrow()
        {
            var builder = _fixture.GetBuilder();
            ValidBuilderSetup(builder);
            builder.Build();
        }

        [Fact]
        public void Build_TwoSerializers_ThrowsInvalidOperation()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            ValidBuilderSetup(builder);

            // Act: Second Serializer
            builder.AddSerializer<ISerializer>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Build_TwoRawMessagePublisher_ThrowsInvalidOperation()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            ValidBuilderSetup(builder);

            // Act
            builder.AddRawMessagePublisher<IRawMessagePublisher>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Build_TwoRawMessageHandlerSubscriber_ThrowsInvalidOperation()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            ValidBuilderSetup(builder);

            // Act
            builder.AddRawMessageHandlerSubscriber<IRawMessageHandlerSubscriber>();

            // Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Constructor_RequiresServicesCollection()
        {
            Assert.Throws<ArgumentNullException>(() => new MessagingBuilder(null));
        }

        private static void ValidBuilderSetup(MessagingBuilder builder)
        {
            builder.AddSerializer<ISerializer>()
                .AddRawMessageHandlerSubscriber<IRawMessageHandlerSubscriber>()
                .AddRawMessagePublisher<IRawMessagePublisher>()
                .AddTopic<MessagingBuilderTests>(nameof(MessagingBuilderTests));
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void AssertRequiredServices(IServiceCollection s, bool publishing, bool subscribing)
        {
            // Services required by the Messaging package:
            // Provided by the application code
            Assert.Equal(1, s.Count(d => d.ServiceType == typeof(ISerializer)));
            Assert.Equal(1, s.Count(d => d.ServiceType == typeof(IMessageTypeTopicMap)));
            // Provided by the builder
            Assert.Equal(1, s.Count(d => d.ServiceType == typeof(MessagingOptions)));

            if (publishing)
            {
                // Provided by the application code
                Assert.Equal(1, s.Count(d => d.ServiceType == typeof(IRawMessagePublisher)));
                // Provided by the builder
                Assert.Equal(1, s.Count(d => d.ServiceType == typeof(IMessagePublisher)));
            }
            if (subscribing)
            {
                // Provided by the application code
                Assert.Equal(1, s.Count(d => d.ServiceType == typeof(IRawMessageHandlerSubscriber)));
                // Provided by the builder
                Assert.Equal(1, s.Count(d => d.ServiceType == typeof(IRawMessageHandler)));
                Assert.Equal(1, s.Count(d => d.ServiceType == typeof(IMessageHandlerInfoProvider)));
            }
        }
    }
}