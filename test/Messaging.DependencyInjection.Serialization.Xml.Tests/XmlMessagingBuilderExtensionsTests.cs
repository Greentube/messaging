using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Serialization;
using Serialization.Xml;
using Xunit;

namespace Messaging.DependencyInjection.Serialization.Xml.Tests
{
    public class XmlMessagingBuilderExtensionsTests
    {
        class Fixture
        {
            public ServiceCollection ServiceCollection { get; set; } = new ServiceCollection();
            public MessagingBuilder GetBuilder() => new MessagingBuilder(ServiceCollection);
        }

        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void AddXml_RegistersXmlSerializer()
        {
            var builder = _fixture.GetBuilder();
            builder.AddXml();

            var descriptor = _fixture.ServiceCollection.FirstOrDefault(d => d.ServiceType == typeof(ISerializer));
            Assert.NotNull(descriptor);

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
            Assert.Equal(typeof(XmlSerializer), descriptor.ImplementationType);
        }
    }
}
