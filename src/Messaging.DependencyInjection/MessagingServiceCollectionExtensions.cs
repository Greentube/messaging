using System;
using Messaging.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MessagingServiceCollectionExtensions
    {
        public static ServiceCollection AddMessaging(
            this ServiceCollection services,
            Action<MessagingBuilder> builderAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var builder = new MessagingBuilder(services);
            builderAction?.Invoke(builder);
            builder.Build();

            return services;
        }
    }
}