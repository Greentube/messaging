using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Messaging
{
    public class MessageHandlerAssemblies : IEnumerable<Assembly>
    {
        private readonly List<Assembly> _asms = new List<Assembly>();

        public void Add(Assembly assembly)
        {
            _asms.Add(assembly);
        }

        public IEnumerator<Assembly> GetEnumerator() => _asms.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}