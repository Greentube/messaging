using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serialization;
using Serialization.Json;
using Xunit;

namespace Messaging.DependencyInjection.Serialization.Json.Tests
{
    public class JsonMessagingBuilderExtensionsTests
    {
        class Fixture
        {
            public ServiceCollection ServiceCollection { get; } = new ServiceCollection();
            public MessagingBuilder GetBuilder() => new MessagingBuilder(ServiceCollection);
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void AddJson_RegistersJsonSerializer()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddJson();

            // Assert
            var descriptor = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ISerializer));
            Assert.NotNull(descriptor);

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(JsonSerializer), descriptor.ImplementationType);

            var JsonOptions =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(JsonOptions));
            Assert.NotNull(JsonOptions);
        }

        [Fact]
        public void AddJson_Action_RegistersJsonSerializer()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            // ReSharper disable once ConvertToLocalFunction - Reference is needed for comparison
            Action<JsonOptions> configAction = _ => { };

            // Act
            builder.AddJson(configAction);

            // Assert
            var optionsConfiguration =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(IConfigureOptions<JsonOptions>));

            Assert.Same(configAction,
                ((ConfigureNamedOptions<JsonOptions>) optionsConfiguration.ImplementationInstance).Action);

            var descriptor = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ISerializer));
            Assert.NotNull(descriptor);

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(JsonSerializer), descriptor.ImplementationType);

            var JsonOptions = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(JsonOptions));
            Assert.NotNull(JsonOptions);
        }
    }
}