using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging
{
    /// <summary>
    /// Provider of type information for <see cref="IMessageHandler"/> implementations
    /// </summary>
    /// <inheritdoc />
    public class MessageHandlerInfoProvider : IMessageHandlerInfoProvider
    {
        private readonly IMessageTypeTopicMap _typeTopicMap;

        /// <summary>
        /// Creates an instance of <see cref="MessageHandlerInfoProvider"/> with the specified <see cref="IMessageTypeTopicMap"/>
        /// </summary>
        /// <param name="typeTopicMap">A map between Topic names and Message types</param>
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
            // Creates a a tuple with the type of handler and message with a delegate to invoke the Handle.
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