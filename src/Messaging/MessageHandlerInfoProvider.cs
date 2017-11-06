using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Messaging
{
    public class MessageHandlerInfoProvider : IMessageHandlerInfoProvider
    {
        private readonly MessageHandlerDiscoveryOptions _options;

        public MessageHandlerInfoProvider(MessageHandlerDiscoveryOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public IEnumerable<(Type messageType, Type handlerType, MethodInfo handleMethod)> GetHandlerInfo()
        {
            return from asm in _options.MessageHandlerAssemblies
                from type in asm.GetTypes()
                let typeInfo = type.GetTypeInfo()
                where typeInfo.IsClass
                      && !typeInfo.IsAbstract
                      && typeInfo.IsPublic || _options.IncludeNonPubicHandlers
                from inf in typeInfo.GetInterfaces().Select(t => t.GetTypeInfo())
                where inf.IsGenericType
                      && inf.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
                let messageType = inf.GetGenericArguments().Single()
                let handleMethod = inf.GetMethod(nameof(IMessageHandler<object>.Handle))
                select (messageType, type, handleMethod);
        }
    }
}