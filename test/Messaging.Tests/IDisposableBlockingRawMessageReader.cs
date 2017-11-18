using System;

namespace Messaging.Tests
{
    public interface IDisposableBlockingRawMessageReader<in TOptions>
        : IDisposable, IBlockingRawMessageReader<TOptions> where TOptions : IPollingOptions
    { }
}