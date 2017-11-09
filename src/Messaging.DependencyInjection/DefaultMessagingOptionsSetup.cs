using System.Reflection;
using Microsoft.Extensions.Options;

namespace Messaging.DependencyInjection
{
    internal class DefaultMessagingOptionsSetup : ConfigureOptions<MessagingOptions>
    {
        public DefaultMessagingOptionsSetup()
            : base(ConfigureOptions)
        {
        }

        private static void ConfigureOptions(MessagingOptions options)
        {
            options.MessageHandlerAssemblies.Add(Assembly.GetEntryAssembly());
        }
    }
}