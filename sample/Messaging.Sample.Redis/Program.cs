using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Messaging.Sample.Redis
{
    public class Program
    {
        static void Main()
        {
            using (var app = new SomeApp(s => s
                .AddMessaging(builder => builder
                        .AddJson()
                        .AddRedis()
                        .AddTopic<SomeMessage>("topic"))
                .AddSingleton<IConnectionMultiplexer>(
                        ConnectionMultiplexer.Connect("localhost:6379"))))
            {
                app.Run();
            } // Graceful shutdown
        }
    }

}