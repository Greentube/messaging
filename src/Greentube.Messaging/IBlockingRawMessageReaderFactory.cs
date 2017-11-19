namespace Greentube.Messaging
{
    /// <summary>
    /// Blocking raw message reader factory
    /// </summary>
    /// <remarks>
    /// Create instances of <see cref="IBlockingRawMessageReader{TOptions}"/>
    /// </remarks>
    /// <typeparam name="TOptions">The polling options</typeparam>
    public interface IBlockingRawMessageReaderFactory<in TOptions>
        where TOptions : IPollingOptions
    {
        /// <summary>
        /// Creates a new reader for the specified topic
        /// </summary>
        /// <param name="topic">The topic to read from</param>
        /// <param name="options">The polling options</param>
        /// <returns><see cref="IBlockingRawMessageReader{TOptions}"/></returns>
        IBlockingRawMessageReader<TOptions> Create(string topic, TOptions options);
    }
}