using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Greentube.Serialization;
using Greentube.Serialization.MessagePack;
using Xunit;

namespace Greentube.Messaging.DependencyInjection.Serialization.MessagePack.Tests
{
    public class MessagePackMessagingBuilderExtensionsTests
    {
        class Fixture
        {
            public ServiceCollection ServiceCollection { get; } = new ServiceCollection();
            public MessagingBuilder GetBuilder() => new MessagingBuilder(ServiceCollection);
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void AddMessagePack_RegistersMessagePackSerializer()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddMessagePack();

            // Assert
            var descriptor = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ISerializer));
            Assert.NotNull(descriptor);

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(MessagePackSerializer), descriptor.ImplementationType);

            var messagePackOptions =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(MessagePackOptions));
            Assert.NotNull(messagePackOptions);
        }

        [Fact]
        public void AddMessagePack_Action_RegistersMessagePackSerializer()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            // ReSharper disable once ConvertToLocalFunction - Reference is needed for comparison
            Action<MessagePackOptions> configAction = _ => { };

            // Act
            builder.AddMessagePack(configAction);

            // Assert
            var optionsConfiguration =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(IConfigureOptions<MessagePackOptions>));

            Assert.NotNull(optionsConfiguration);
            Assert.Same(configAction,
                ((ConfigureNamedOptions<MessagePackOptions>) optionsConfiguration.ImplementationInstance).Action);

            var descriptor = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ISerializer));
            Assert.NotNull(descriptor);

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(MessagePackSerializer), descriptor.ImplementationType);

            var messagePackOptions = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(MessagePackOptions));
            Assert.NotNull(messagePackOptions);
        }
    }
}