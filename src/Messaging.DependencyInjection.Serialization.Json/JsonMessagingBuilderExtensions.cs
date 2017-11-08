using System;
using Messaging.DependencyInjection;
using Microsoft.Extensions.Options;
using Serialization.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonMessagingBuilderExtensions
    {
        public static MessagingBuilder AddJson(this MessagingBuilder builder)
        {
            builder.AddSerializer<JsonConvertSerializer>();
            builder.Services.AddSingleton(c => c.GetRequiredService<IOptions<JsonOptions>>().Value);

            return builder;
        }

        public static MessagingBuilder AddJson(this MessagingBuilder builder, Action<JsonOptions> setupAction)
        {
            builder.Services.Configure(setupAction);
            return builder.AddJson();
        }
    }
}