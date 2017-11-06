using System;
using Messaging.DependencyInjection;
using Serialization.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonMessagingBuilderExtensions
    {
        public static MessagingBuilder AddJson(this MessagingBuilder builder)
        {
            builder.AddSerializer<JsonConvertSerializer>();
            return builder;
        }

        public static MessagingBuilder AddJson(this MessagingBuilder builder, Action<JsonOptions> setupAction)
        {
            builder.Services.Configure(setupAction);
            return builder.AddJson();
        }
    }
}