using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Greentube.Messaging.DependencyInjection
{
    /// <summary>
    /// Type discovery settings
    /// </summary>
    /// <remarks>
    /// This is no a 'IOptions' class. It drives the behavior of the registration of services
    /// </remarks>
    public class DiscoverySettings
    {
        /// <summary>
        /// Assemblies to scan for implementations of <see cref="IMessageHandler"/>
        /// </summary>
        public ISet<Assembly> MessageHandlerAssemblies { get; } = new HashSet<Assembly>
        {
            Assembly.GetEntryAssembly()
        };
        /// <summary>
        /// The lifetime to define all services found via assembly scan
        /// </summary>
        public ServiceLifetime DiscoveredHandlersLifetime { get; set; } = ServiceLifetime.Transient;
        /// <summary>
        /// Include implementations of <see cref="IMessageHandler"/> with non 'public' access modifier
        /// </summary>
        public bool IncludeNonPublic { get; set; }
        /// <summary>
        /// How to handle cases where the service is already registered
        /// </summary>
        /// <remarks>
        /// When discovering handlers in assemblies, it's possible the handler has already been registered.
        /// That's the case when the application code has explicitly registered it with custom Lifetime or some decorator pattern.
        /// This setting defines what to do in these cases. By default, the discovered handler will NOT be overwriten by the discovered one.
        /// </remarks>
        internal RegistrationStrategy RegistrationStrategy { get; set; } = RegistrationStrategy.Skip;
    }
}