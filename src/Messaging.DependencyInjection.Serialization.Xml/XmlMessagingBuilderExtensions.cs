using System;
using Messaging.DependencyInjection;
using Microsoft.Extensions.Options;
using Serialization.Xml;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class XmlMessagingBuilderExtensions
    {
        public static MessagingBuilder AddXml(this MessagingBuilder builder)
        {
            builder.AddSerializer<XmlSerializer>();
            builder.Services.AddSingleton(c => c.GetRequiredService<IOptions<XmlOptions>>().Value);
            return builder;
        }

        public static MessagingBuilder AddXml(this MessagingBuilder builder, Action<XmlOptions> setupAction)
        {
            builder.Services.Configure(setupAction);
            return builder.AddXml();
        }
    }
}