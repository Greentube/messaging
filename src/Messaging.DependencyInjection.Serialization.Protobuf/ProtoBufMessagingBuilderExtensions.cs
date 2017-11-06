using System;
using Messaging.DependencyInjection;
using Serialization.Protobuf;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProtoBufMessagingBuilderExtensions
    {
        public static MessagingBuilder AddProtoBuf(this MessagingBuilder builder)
        {
            builder.AddSerializer<ProtoBufSerializer>();
            return builder;
        }

        public static MessagingBuilder AddProtoBuf(this MessagingBuilder builder, Action<ProtoBufOptions> setupAction)
        {
            builder.Services.Configure(setupAction);
            return builder.AddProtoBuf();
        }
    }
}