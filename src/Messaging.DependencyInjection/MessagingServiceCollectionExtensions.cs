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
            services.AddSingleton(c => c.GetRequiredService<IOptions<MessagingOptions>>().Value);

            // Add Messaging services
            services.TryAddSingleton<IMessagePublisher, SerializedMessagePublisher>();
            services.TryAddSingleton<IRawMessageHandler, DispatchingRawMessageHandler>();
            services.TryAddSingleton<IMessageHandlerInfoProvider, MessageHandlerInfoProvider>();
            services.TryAddSingleton<IMessageHandlerInvoker>(c =>
                new MessageHandlerInvoker(
                    c.GetService<IMessageHandlerInfoProvider>(),
                    c.GetService));

            services.TryAddSingleton(p =>
            {
                var messagingOptions = p.GetRequiredService<MessagingOptions>();
                return messagingOptions.MessageHandlerAssemblies;
            });

            var builder = new MessagingBuilder(services);
            builderAction?.Invoke(builder);
            builder.Build();

            return services;
        }
    }
}