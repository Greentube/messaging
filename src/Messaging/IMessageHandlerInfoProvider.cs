using System;
using System.Collections.Generic;
using System.Reflection;

namespace Messaging
{
    /// <summary>
    /// Provider of <see cref="IMessageHandler"/> reflection data.
    /// </summary>
    public interface IMessageHandlerInfoProvider
    {
        IEnumerable<(Type messageType, Type handlerType, MethodInfo handleMethod)> GetHandlerInfo();
    }
}