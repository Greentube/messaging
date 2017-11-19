namespace Greentube.Messaging
{
    /// <summary>
    /// Raw message reader which doesn't support TPL and hence blocks on I/O
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public interface IBlockingRawMessageReader<in TOptions>
        where TOptions : IPollingOptions
    {
        /// <summary>
        /// Tries to read a message from the bus
        /// </summary>
        /// <param name="message">The message read if true was returned</param>
        /// <param name="options">options</param>
        /// <returns><c>true</c> when a message is successfully read. Otherwise <c>false</c></returns>
        bool TryGetMessage(out byte[] message, TOptions options);
    }
}