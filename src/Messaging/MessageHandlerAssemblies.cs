using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Messaging
{
    public class MessageHandlerAssemblies : IEnumerable<Assembly>
    {
        private readonly HashSet<Assembly> _asms = new HashSet<Assembly>();

        public void Add(Assembly assembly)
        {
            _asms.Add(assembly);
        }

        public IEnumerator<Assembly> GetEnumerator() => _asms.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(IEnumerable<Assembly> assemblies)
        {
            foreach (var asm in assemblies)
            {
                _asms.Add(asm);
            }
        }
    }
}