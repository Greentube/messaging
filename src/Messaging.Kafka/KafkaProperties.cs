using System.Collections.Generic;

namespace Messaging.Kafka
{
    /// <summary>
    /// The key-value pair of configurations for Apache Kafka
    /// </summary>
    /// <inheritdoc />
    public class KafkaProperties : Dictionary<string, object>
    {
        public string ClientId
        {
            get => (string) this["client.id"];
            set => this["client.id"] = value;
        }

        public string AutoOffset
        {
            get => (string) this["auto.offset.reset"];
            set => this["auto.offset.reset"] = value;
        }

        public int BatchSize
        {
            get => (int) this["batch.num.messages"];
            set => this["batch.num.messages"] = value;
        }

        public string BrokerList
        {
            get => (string) this["bootstrap.servers"];
            set => this["bootstrap.servers"] = value;
        }

        public string GroupId
        {
            get => (string) this["group.id"];
            set => this["group.id"] = value;
        }

        public int QueueBufferSize
        {
            get => (int) this["queue.buffering.max.messages"];
            set => this["queue.buffering.max.messages"] = value;
        }

        public int QueueBufferTime
        {
            get => (int) this["queue.buffering.max.ms"];
            set => this["queue.buffering.max.ms"] = value;
        }

        public int FetchWaitTime
        {
            get => (int) this["fetch.wait.max.ms"];
            set => this["fetch.wait.max.ms"] = value;
        }
    }
}