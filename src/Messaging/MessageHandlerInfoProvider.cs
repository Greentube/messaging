using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Messaging
{
    public class MessageHandlerInfoProvider : IMessageHandlerInfoProvider
    {
        private readonly MessageHandlerDiscoveryOptions _discoveryOptions;

        public MessageHandlerInfoProvider(MessagingOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _discoveryOptions = options.DiscoveryOptions;

            if (!_discoveryOptions?.MessageHandlerAssemblies?.Any() ?? false)
                throw new ArgumentException(
                    $"{nameof(MessagingOptions)} have no available {nameof(MessageHandlerAssemblies)} defined.");
        }

        public IEnumerable<(Type messageType, Type handlerType, MethodInfo handleMethod)> GetHandlerInfo()
        {
            return from asm in _discoveryOptions.MessageHandlerAssemblies
                from type in asm.GetTypes()
                let typeInfo = type.GetTypeInfo()
                where typeInfo.IsClass
                      && !typeInfo.IsAbstract
                      && typeInfo.IsPublic || _discoveryOptions.IncludeNonPubicHandlers
                from inf in typeInfo.GetInterfaces().Select(t => t.GetTypeInfo())
                where inf.IsGenericType
                      && inf.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
                let messageType = inf.GetGenericArguments().Single()
                let handleMethod = inf.GetMethod(nameof(IMessageHandler<object>.Handle))
                select (messageType, type, handleMethod);
        }
    }
}