using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Messaging
{
    /// <summary>
    /// A list of assemblies to search for implementations of <see cref="IMessageHandler"/>
    /// </summary>
    /// <inheritdoc />
    public class MessageHandlerAssemblies : IEnumerable<Assembly>
    {
        private readonly HashSet<Assembly> _asms = new HashSet<Assembly>();

        /// <summary>
        /// Add an assembly to look for implementations of <see cref="IMessageHandler"/>
        /// </summary>
        /// <param name="assembly"></param>
        public void Add(Assembly assembly)
        {
            _asms.Add(assembly);
        }
        
        /// <summary>
        /// Add a range of Assemblies to look up implementations of <see cref="IMessageHandler"/>
        /// </summary>
        /// <param name="assemblies"></param>
        public void AddRange(IEnumerable<Assembly> assemblies)
        {
            foreach (var asm in assemblies)
            {
                _asms.Add(asm);
            }
        }

        /// <inheritdoc />
        public IEnumerator<Assembly> GetEnumerator() => _asms.GetEnumerator();
        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}