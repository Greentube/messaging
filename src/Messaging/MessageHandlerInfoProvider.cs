using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Messaging
{
    public class MessageHandlerInfoProvider : IMessageHandlerInfoProvider
    {
        private readonly MessageHandlerAssemblies _handlerAssemblies;

        public MessageHandlerInfoProvider(MessageHandlerAssemblies handlerAssemblies)
        {
            _handlerAssemblies = handlerAssemblies ?? throw new ArgumentNullException(nameof(handlerAssemblies));

            if (!_handlerAssemblies.Any())
                throw new ArgumentException(
                    $"{nameof(MessagingOptions)} has no available {nameof(MessageHandlerAssemblies)} defined.");
        }

        public IEnumerable<(Type messageType, Type handlerType, MethodInfo handleMethod)> GetHandlerInfo()
        {
            return from asm in _handlerAssemblies
                from type in asm.GetTypes()
                let typeInfo = type.GetTypeInfo()
                where typeInfo.IsClass
                      && !typeInfo.IsAbstract
                from inf in typeInfo.GetInterfaces().Select(t => t.GetTypeInfo())
                where inf.IsGenericType
                      && inf.GetGenericTypeDefinition() == typeof(IMessageHandler<>)
                let messageType = inf.GetGenericArguments().Single()
                let handleMethod = inf.GetMethod(nameof(IMessageHandler<object>.Handle))
                select (messageType, type, handleMethod);
        }
    }
}