using System;
using Messaging.DependencyInjection;
using Microsoft.Extensions.Options;
using Serialization.MessagePack;

// ReSharper disable once CheckNamespace - Discoverability
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions to <see cref="MessageBuilder"/> to add <see cref="MessagePackSerializer"/>
    /// </summary>
    public static class MessagePackMessagingBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="MessagePackSerializer"/> to <see cref="MessagingBuilder"/>
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <returns>Messaging builder</returns>
        public static MessagingBuilder AddMessagePack(this MessagingBuilder builder)
        {
            builder.AddSerializer<MessagePackSerializer>();
            builder.Services.AddSingleton(c => c.GetRequiredService<IOptions<MessagePackOptions>>().Value);
            return builder;
        }

        /// <summary>
        /// Adds <see cref="MessagePackSerializer"/> to <see cref="MessagingBuilder"/>
        /// </summary>
        /// <param name="builder">Messaging builder</param>
        /// <param name="setupAction">Action to setup <see cref="MessagePackOptions"/></param>
        /// <returns>Messaging builder</returns>
        public static MessagingBuilder AddMessagePack(this MessagingBuilder builder, Action<MessagePackOptions> setupAction)
        {
            builder.Services.Configure(setupAction);
            return builder.AddMessagePack();
        }
    }
}