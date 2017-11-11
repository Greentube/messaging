using System.Linq;
using Messaging.Redis;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using StackExchange.Redis;
using Xunit;

namespace Messaging.DependencyInjection.Redis.Tests
{
    public class RedisMessagingBuilderExtensionsTests
    {
        class Fixture
        {
            public ServiceCollection ServiceCollection { get; } = new ServiceCollection();
            public MessagingBuilder GetBuilder() => new MessagingBuilder(ServiceCollection);
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void AddRedis_RegistersBothPublisherAndSubscriber()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddRedis();

            // Assert
            AssertPublisher();
            AssertSubscriber();
        }

        [Fact]
        public void AddRedis_Multiplexer_RegistersPublisherSubscriberAndMultiplexer()
        {
            // Arrange
            var multiplexer = Substitute.For<IConnectionMultiplexer>();
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddRedis(multiplexer);

            // Assert
            var multiplexerDescriptor =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(IConnectionMultiplexer));
            Assert.NotNull(multiplexerDescriptor);

            Assert.Equal(ServiceLifetime.Singleton, multiplexerDescriptor.Lifetime);
            Assert.Same(multiplexer, multiplexerDescriptor.ImplementationInstance);

            AssertPublisher();
            AssertSubscriber();
        }

        [Fact]
        public void AddRedisPublisher_RegisterOnlyPublisher()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddRedisPublisher();

            // Assert
            AssertPublisher();
            Assert.DoesNotContain(_fixture.ServiceCollection, d => d.ServiceType == typeof(IRawMessageHandlerSubscriber));
        }

        [Fact]
        public void AddRedisSubscriber_RegisterOnlySubscriber()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddRedisSubscriber();

            // Assert
            AssertSubscriber();
            Assert.DoesNotContain(_fixture.ServiceCollection, d => d.ServiceType == typeof(IRawMessagePublisher));
        }


        private void AssertSubscriber()
        {
            var subscriber =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(IRawMessageHandlerSubscriber));
            Assert.NotNull(subscriber);

            Assert.Equal(ServiceLifetime.Singleton, subscriber.Lifetime);
            Assert.Equal(typeof(RedisRawMessageHandlerSubscriber), subscriber.ImplementationType);
        }

        private void AssertPublisher()
        {
            var publisher = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(IRawMessagePublisher));
            Assert.NotNull(publisher);

            Assert.Equal(ServiceLifetime.Singleton, publisher.Lifetime);
            Assert.Equal(typeof(RedisRawMessagePublisher), publisher.ImplementationType);
        }
    }
}