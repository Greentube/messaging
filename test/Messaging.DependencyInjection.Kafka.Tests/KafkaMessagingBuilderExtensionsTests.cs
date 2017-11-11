using System;
using System.Linq;
using Messaging.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Messaging.DependencyInjection.Kafka.Tests
{
    public class KafkaMessagingBuilderExtensionsTests
    {
        class Fixture
        {
            public ServiceCollection ServiceCollection { get; } = new ServiceCollection();
            public MessagingBuilder GetBuilder() => new MessagingBuilder(ServiceCollection);
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void AddKafka_RegistersPublisherSubscriberAndOptions()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddKafka();

            // Assert
            AssertPublisher();
            AssertSubscriber();
            AssertOptions();
        }

        [Fact]
        public void AddKafka_Action_RegistersPublisherSubscriberOptionsAndSetupAction()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            // ReSharper disable once ConvertToLocalFunction - Reference is needed for comparison
            Action<KafkaOptions> configAction = _ => { };

            // Act
            builder.AddKafka(configAction);

            // Assert
            AssertOptionSetup(configAction);
            AssertPublisher();
            AssertSubscriber();
            AssertOptions();
        }

        [Fact]
        public void AddKafkaPublisher_RegisterOnlyPublisher()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddKafkaPublisher();

            // Assert
            AssertPublisher();
            AssertOptions();
            Assert.DoesNotContain(_fixture.ServiceCollection, d => d.ServiceType == typeof(IRawMessageHandlerSubscriber));
        }

        [Fact]
        public void AddKafkaPublisher_Action_RegisterOnlyPublisherAndSetupAction()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            // ReSharper disable once ConvertToLocalFunction - Reference is needed for comparison
            Action<KafkaOptions> configAction = _ => { };

            // Act
            builder.AddKafkaPublisher(configAction);

            // Assert
            AssertOptionSetup(configAction);
            AssertPublisher();
            AssertOptions();
            Assert.DoesNotContain(_fixture.ServiceCollection, d => d.ServiceType == typeof(IRawMessageHandlerSubscriber));
        }

        [Fact]
        public void AddKafkaSubscriber_RegisterOnlySubscriber()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddKafkaSubscriber();

            // Assert
            AssertOptions();
            AssertSubscriber();
            Assert.DoesNotContain(_fixture.ServiceCollection, d => d.ServiceType == typeof(IRawMessagePublisher));
        }

        [Fact]
        public void AddKafkaSubscriber_Action_RegisterOnlySubscriberAndSetupAction()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            // ReSharper disable once ConvertToLocalFunction - Reference is needed for comparison
            Action<KafkaOptions> configAction = _ => { };

            // Act
            builder.AddKafkaSubscriber(configAction);

            // Assert
            AssertOptionSetup(configAction);
            AssertSubscriber();
            AssertOptions();
            Assert.DoesNotContain(_fixture.ServiceCollection, d => d.ServiceType == typeof(IRawMessagePublisher));
        }

        private void AssertSubscriber()
        {
            var subscriber =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(IRawMessageHandlerSubscriber));
            Assert.NotNull(subscriber);

            Assert.Equal(ServiceLifetime.Singleton, subscriber.Lifetime);
            Assert.Equal(typeof(KafkaRawMessageHandlerSubscriber), subscriber.ImplementationType);
        }

        private void AssertPublisher()
        {
            var publisher =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(IRawMessagePublisher));
            Assert.NotNull(publisher);

            Assert.Equal(ServiceLifetime.Singleton, publisher.Lifetime);
            Assert.Equal(typeof(KafkaRawMessagePublisher), publisher.ImplementationType);
        }

        private void AssertOptions()
        {
            var options =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(KafkaOptions));
            Assert.NotNull(options);
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void AssertOptionSetup(Action<KafkaOptions> configAction)
        {
            var optionsConfiguration =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(IConfigureOptions<KafkaOptions>));

            Assert.NotNull(optionsConfiguration);
            Assert.Same(configAction,
                ((ConfigureNamedOptions<KafkaOptions>) optionsConfiguration.ImplementationInstance).Action);
        }
    }
}