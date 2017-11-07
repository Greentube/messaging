﻿using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Messaging.Redis
{
    public class RedisRawMessagePublisher : IRawMessagePublisher
    {
        private readonly ISubscriber _subscriber;

        public RedisRawMessagePublisher(IConnectionMultiplexer mux) =>
            _subscriber = mux?.GetSubscriber() ?? throw new ArgumentException("No subscription available.", nameof(mux));

        public Task Publish(string topic, byte[] message, CancellationToken _)
        {
            return _subscriber.PublishAsync(topic, message);
        }
    }
}