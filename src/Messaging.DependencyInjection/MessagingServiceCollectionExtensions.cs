using System;
using Messaging;
using Messaging.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MessagingServiceCollectionExtensions
    {
        public static ServiceCollection AddMessaging(
            this ServiceCollection services,
            Action<MessagingBuilder> builderAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MessagingOptions>, DefaultMessagingOptionsSetup>());

            // Add Messaging services
            services.TryAddSingleton<IMessagePublisher, SerializedMessagePublisher>();
            services.TryAddSingleton<IRawMessageHandler, DispatchingRawMessageHandler>();
            services.TryAddSingleton<IMessageHandlerInvoker>(c =>
                new MessageHandlerInvoker(
                    c.GetService<IMessageHandlerInfoProvider>(),
                    c.GetService));

            var builder = new MessagingBuilder(services);
            builderAction?.Invoke(builder);
            builder.Build();

            return services;
        }
    }
}