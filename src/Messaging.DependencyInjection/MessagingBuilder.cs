using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serialization;

namespace Messaging.DependencyInjection
{
    public class MessagingBuilder
    {
        public IServiceCollection Services { get; }

        internal MessagingBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public void ConfigureOptions(Action<MessagingOptions> options)
        {
            Services.Configure(options);
        }

        public void AddSerializer<TSerializer>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TSerializer : ISerializer
        {
            Services.Add(ServiceDescriptor.Describe(typeof(ISerializer),typeof(TSerializer),lifetime));
        }

        public void Build()
        {
            var serializers = Services.Count(s => s.ServiceType == typeof(ISerializer));
            if (serializers == 0)
            {
                throw new InvalidOperationException("No serializer has been configured.");
            }
            if (serializers > 1)
            {
                throw new InvalidOperationException("More than one serializer has been configured. Please define a single serializer.");
            }
        }
    }
}