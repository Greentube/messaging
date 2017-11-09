using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging
{
    public class MessageHandlerInfoProvider : IMessageHandlerInfoProvider
    {
        private readonly IMessageTypeTopicMap _typeTopicMap;

        public MessageHandlerInfoProvider(IMessageTypeTopicMap typeTopicMap)
        {
            _typeTopicMap = typeTopicMap ?? throw new ArgumentNullException(nameof(typeTopicMap));

            if (!_typeTopicMap.Any())
                throw new ArgumentException($"{nameof(IMessageTypeTopicMap)} is empty.");
        }

        public IEnumerable<(
                Type messageType,
                Type handlerType,
                Func<object, object, CancellationToken, Task> handleMethod)>
            GetHandlerInfo()
        {
            return from messageType in _typeTopicMap.GetMessageTypes()
                let handlerType = typeof(IMessageHandler<>).MakeGenericType(messageType)
                let handleMethod = handlerType.GetTypeInfo().GetMethod(nameof(IMessageHandler<object>.Handle))
                let handlerInstance = Expression.Parameter(typeof(object))
                let messageInstance = Expression.Parameter(typeof(object))
                let tokenInstance = Expression.Parameter(typeof(CancellationToken))
                let handleFunc = Expression.Lambda<Func<object, object, CancellationToken, Task>>(
                    Expression.Call(
                        Expression.Convert(
                            handlerInstance,
                            handlerType),
                        handleMethod,
                        Expression.Convert(
                            messageInstance,
                            messageType),
                        tokenInstance),
                    handlerInstance,
                    messageInstance,
                    tokenInstance)
                    .Compile()
                select (messageType, handlerType, handleFunc);
        }
    }
}