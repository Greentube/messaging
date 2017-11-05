using System;
using System.Collections.Generic;
using System.Reflection;

namespace Messaging
{
    public interface IMessageHandlerInfoProvider
    {
        IEnumerable<(Type messageType, Type handlerType, MethodInfo handleMethod)> GetHandlerInfo();
    }
}