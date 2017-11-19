using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Greentube.Serialization;
using Greentube.Serialization.Xml;
using Xunit;

namespace Greentube.Messaging.DependencyInjection.Serialization.Xml.Tests
{
    public class XmlMessagingBuilderExtensionsTests
    {
        class Fixture
        {
            public ServiceCollection ServiceCollection { get; } = new ServiceCollection();
            public MessagingBuilder GetBuilder() => new MessagingBuilder(ServiceCollection);
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void AddXml_RegistersXmlSerializer()
        {
            // Arrange
            var builder = _fixture.GetBuilder();

            // Act
            builder.AddXml();

            // Assert
            var descriptor = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ISerializer));
            Assert.NotNull(descriptor);

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(XmlSerializer), descriptor.ImplementationType);

            var xmlOptions = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(XmlOptions));
            Assert.NotNull(xmlOptions);
        }

        [Fact]
        public void AddXml_Action_RegistersXmlSerializer()
        {
            // Arrange
            var builder = _fixture.GetBuilder();
            // ReSharper disable once ConvertToLocalFunction - Reference is needed for comparison
            Action<XmlOptions> configAction = _ => { };

            // Act
            builder.AddXml(configAction);

            // Assert
            var optionsConfiguration =
                _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(IConfigureOptions<XmlOptions>));

            Assert.NotNull(optionsConfiguration);
            Assert.Same(configAction,
                ((ConfigureNamedOptions<XmlOptions>) optionsConfiguration.ImplementationInstance).Action);

            var descriptor = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ISerializer));
            Assert.NotNull(descriptor);

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(XmlSerializer), descriptor.ImplementationType);

            var xmlOptions = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(XmlOptions));
            Assert.NotNull(xmlOptions);
        }
    }
}