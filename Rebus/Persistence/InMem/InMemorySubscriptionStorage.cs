﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rebus.Subscriptions;

namespace Rebus.Persistence.InMem
{
    public class InMemorySubscriptionStorage : ISubscriptionStorage
    {
        static readonly StringComparer StringComparer = StringComparer.InvariantCultureIgnoreCase;

        readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object>> _subscribers
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, object>>(StringComparer);

        public async Task<IEnumerable<string>> GetSubscriberAddresses(string topic)
        {
            ConcurrentDictionary<string, object> subscriberAddresses;

            return _subscribers.TryGetValue(topic, out subscriberAddresses)
                ? subscriberAddresses.Keys
                : Enumerable.Empty<string>();
        }

        public async Task RegisterSubscriber(string topic, string subscriberAddress)
        {
            _subscribers.GetOrAdd(topic, _ => new ConcurrentDictionary<string, object>(StringComparer))
                .TryAdd(subscriberAddress, new object());
        }

        public async Task UnregisterSubscriber(string topic, string subscriberAddress)
        {
            object dummy;

            _subscribers.GetOrAdd(topic, _ => new ConcurrentDictionary<string, object>(StringComparer))
                .TryRemove(subscriberAddress, out dummy);
        }

        public bool IsCentralized
        {
            get
            {
                // in-mem subscription storage is decentralized
                return false;
            }
        }
    }
}