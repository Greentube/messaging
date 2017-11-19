using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Greentube.Serialization;
using Greentube.Serialization.ProtoBuf;
using Xunit;

namespace Greentube.Messaging.DependencyInjection.Serialization.ProtoBuf.Tests
{
    public class ProtoBufMessagingBuilderExtensionsTests
    {
        class Fixture
        {
            public ServiceCollection ServiceCollection { get; } = new ServiceCollection();
            public MessagingBuilder GetBuilder() => new MessagingBuilder(ServiceCollection);
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void AddProtoBuf_RegistersProtoBufSerializer()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddProtoBuf();

            // Assert
            var descriptor = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ISerializer));
            Assert.NotNull(descriptor);

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(ProtoBufSerializer), descriptor.ImplementationType);

            var protoBufOptions =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ProtoBufOptions));
            Assert.NotNull(protoBufOptions);
        }

        [Fact]
        public void AddProtoBuf_Action_RegistersProtoBufSerializer()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            // ReSharper disable once ConvertToLocalFunction - Reference is needed for comparison
            Action<ProtoBufOptions> configAction = _ => { };

            // Act
            builder.AddProtoBuf(configAction);

            // Assert
            var optionsConfiguration =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(IConfigureOptions<ProtoBufOptions>));

            Assert.NotNull(optionsConfiguration);
            Assert.Same(configAction,
                ((ConfigureNamedOptions<ProtoBufOptions>) optionsConfiguration.ImplementationInstance).Action);

            var descriptor = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ISerializer));
            Assert.NotNull(descriptor);

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(ProtoBufSerializer), descriptor.ImplementationType);

            var protoBufOptions = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ProtoBufOptions));
            Assert.NotNull(protoBufOptions);
        }
    }
}