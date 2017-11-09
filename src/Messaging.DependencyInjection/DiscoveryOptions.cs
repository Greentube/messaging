using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Messaging.DependencyInjection
{
    public class DiscoverySettings
    {
        public MessageHandlerAssemblies MessageHandlerAssemblies { get; } = new MessageHandlerAssemblies
        {
            Assembly.GetEntryAssembly()
        };
        public ServiceLifetime DiscoveredHandlersLifetime { get; set; } = ServiceLifetime.Transient;
        public bool IncludeNonPublic { get; set; }
        internal RegistrationStrategy RegistrationStrategy { get; set; } = RegistrationStrategy.Skip;
    }
}