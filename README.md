# messaging
An opinionated messaging library for simple pub/sub with different serialization and middlewares.

## Publishing a message on an ASP.NET Core application
```csharp

// Class without annotations
public class SomeMessage
{
    public string Body { get; set; }
}

// Handling messages: Class implementing IMessageHandler<T> to be invoked when T arrives
internal class SomeMessageHandler : IMessageHandler<SomeMessage>
{
    public Task Handle(SomeMessage message, CancellationToken _)
    {
        Console.WriteLine($"Handled: {message}.");
        return Task.CompletedTask;
    }
}

// Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IMessageHandler<SomeMessage>, SomeMessageHandler>();
    services.AddMessaging(o => 
    {
        o.AddRedis();
        o.AddProtobuf();
        o.MessageTypeTopicMap.Add(typeof(SomeMessage), "SomeTopic");
    });
}
public void Configure(IApplicationBuilder app)
{
    app.UseMessagingSubscriptions(); // Subscribes to the topics defined via Services
}

// Publishing via ASP.NET Core MVC:
[Route("some-message")]
public class SomeMessageController
{
    private readonly IMessagePublisher _publisher;
    public SomeMessageController(IMessagePublisher publisher) => _publisher;

    [HttpPut]
    public async Task<IActionResult> PublishSomeMessage([FromBody] SomeMessage message, CancellationToken token)
    {
        await _publisher.Publish(message, token);
        return Accepted();
    }
}

```
