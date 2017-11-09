using System;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Sample.Kafka
{
    public class Program
    {
        static void Main()
        {
            using (var app = new SomeApp(s =>
                s.AddMessaging(builder =>
                {
                    builder.AddJson();
                    builder.AddKafka(o =>
                    {
                        o.Properties.BrokerList = "localhost:9092";
                        o.Properties.GroupId = "Sample-GroupId";
                        o.Properties.Add("socket.timeout.ms", 1000); // sample unmapped setting
                        o.Subscriber.ConsumerCreatedCallback =
                            consumer => consumer.OnError += (sender, error)
                                => Console.WriteLine($"Consumer error: ${error}");
                    });
                })))
            {
                app.Run();
            } // Graceful shutdown
        }
    }


}