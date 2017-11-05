namespace Messaging
{
    public class MessageHandlerDiscoveryOptions
    {
        public MessageHandlerAssemblies MessageHandlerAssemblies { get; } = new MessageHandlerAssemblies();
        public bool IncludeNonPubicHandlers { get; set; }
    }
}