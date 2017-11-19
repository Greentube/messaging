using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Messaging
{
    /// <summary>
    /// Provider of <see cref="IMessageHandler"/> reflection data.
    /// </summary>
    public interface IMessageHandlerInfoProvider
    {
        IEnumerable<(Type messageType, Type handlerType, Func<object, object, CancellationToken, Task>  handleMethod)> GetHandlerInfo();
    }
}