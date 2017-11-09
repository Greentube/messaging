using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Messaging
{
    ///<inheritdoc />
    public class MessageHandlerInvoker : IMessageHandlerInvoker
    {
        private readonly Dictionary<
                Type,
                (Type handlerType, Func<object, object, CancellationToken, Task> handleMethod)>
                _messageToHandlerDictionary;

        private readonly Func<Type, object> _handlerFactory;

        public MessageHandlerInvoker(
            IMessageHandlerInfoProvider messageHandlerInfoProvider,
            Func<Type, object> handlerFactory)
        {
            if (messageHandlerInfoProvider == null) throw new ArgumentNullException(nameof(messageHandlerInfoProvider));
            _handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));

            _messageToHandlerDictionary = messageHandlerInfoProvider.GetHandlerInfo()
                .ToDictionary(
                    k => k.messageType,
                    v => (v.handlerType, v.handleMethod));

            if (!_messageToHandlerDictionary.Any())
                throw new InvalidOperationException(
                    $"{nameof(IMessageHandlerInfoProvider)}.{nameof(IMessageHandlerInfoProvider.GetHandlerInfo)} " +
                    "hasn't resolved any handler information.");
        }

        ///<inheritdoc />
        public Task Invoke(object message, CancellationToken token)
        {
            if (!_messageToHandlerDictionary.TryGetValue(message.GetType(), out var handlerInfo))
            {
                throw new InvalidOperationException($"No message handler found for message type {message.GetType()}.");
            }

            var handler = _handlerFactory(handlerInfo.handlerType);
            if (handler == null) throw new InvalidOperationException($"Message of type {message.GetType()} yielded no handler from factory.");

            return handlerInfo.handleMethod(handler, message, token);
        }
    }
}