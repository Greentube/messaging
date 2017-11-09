# messaging
An opinionated messaging library for simple pub/sub with different serialization and message broker/middleware.

To use this library, you need to decide on which [serialization](#serialization) 
and [messaging middleware](#messaging-middleware).


### Publishing and Handling a message on an ASP.NET Core application
```csharp

// Class without annotations
public class SomeMessage
{
    public string Body { get; set; }
}

// Handling messages: Class implementing IMessageHandler<T> to be invoked when T arrives
public class SomeMessageHandler : IMessageHandler<SomeMessage>
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
    services.AddMessaging(builder =>
    {
        builder.AddProtoBuf();
        builder.AddRedis();
        builder.MessageTypeTopicMap.Add(typeof(SomeMessage), "SomeTopic");
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

#### Messaging middleware
The current supported messaging systems are:

* [Redis](https://redis.io/topics/pubsub)
* [Apache Kafka](https://kafka.apache.org/)


#### Serialization
The supported serialization methods are:

* JSON - with Newtonsoft.Json
* XML - with System.Xml.XmlSerializer
* Protobuf - with protobuf-net

Example serialization setup:

```csharp
services.AddMessaging(builder =>
{
    builder.AddProtoBuf();
    // or
    builder.AddXml();
    // or
    builder.AddJson();
});
```

Each implementation has some additional settings

##### JSON

Define the encoding.

```csharp
// Use UTF-16 instead of the default UTF-8
builder.AddJson(o => o.Encoding = Encoding.Unicode);
```

##### XML

Xml with user-defined default namespace
```csharp
builder.AddXml(p => p.DefaultNamespace = "some-namespace");
```
Xml with user-defined factory delegate
```csharp 
// Root attribute will be named: 'messaging'
builder.AddXml(p => p.Factory = type => new XmlSerializer(type, new XmlRootAttribute("messaging")));
```

##### Protobuf

Custom RuntimeTypeModel
```csharp
var model = RuntimeTypeModel.Create();
model.Add(typeof(SomeMessage), false).Add(1, nameof(SomeMessage.Body));
builder.AddProtoBuf(o => o.RuntimeTypeModel = runtimeTypeModel);
```

#### Discovering and Invoking handlers

By default. Implementations of `MessageHandler<T>` are discovered and registered dynamically. 
When classes are in assemblies other than the entry assembly, the library might need some help to find them. Similar to the [MVC Application parts](https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/app-parts) concept.

Only `public` implementations of `IMessageHandler<T>` will be automatically registered by default.
 
The lifetime of the automatically registered Handlers is `Transient`. Each time a message arrives, the library will ask the container
for a message handler. Something like: `provider.GetService<IMessageHandler<T>>();` for each call.
It's advisable you switch to Singleton lifetime in case your handlers are thread-safe.

The characteristic mentioned above can be customized with:
```csharp
builder.AddHandlerDiscovery(d =>
    {
        d.IncludeNonPublic = true;
        d.DiscoveredHandlersLifetime = ServiceLifetime.Singleton;
        d.MessageHandlerAssemblies.Add(typeof(SomeMessage).Assembly);
    });
``` 


