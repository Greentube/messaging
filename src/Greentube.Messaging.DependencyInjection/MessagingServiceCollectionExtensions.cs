using System;
using Greentube.Messaging.DependencyInjection;

// ReSharper disable once CheckNamespace - Discoverability
namespace Microsoft.Extensions.DependencyInjection
{
    public static class MessagingServiceCollectionExtensions
    {
        /// <summary>
        /// Add Messaging services
        /// </summary>
        /// <param name="services">ServicesCollection</param>
        /// <param name="builderAction">Action to handle the messaging builder</param>
        /// <returns>ServicesCollection</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ServiceCollection AddMessaging(
            this ServiceCollection services,
            Action<MessagingBuilder> builderAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (builderAction == null) throw new ArgumentNullException(nameof(builderAction));

            var builder = new MessagingBuilder(services);
            builderAction.Invoke(builder);
            builder.Build();

            return services;
        }
    }
}