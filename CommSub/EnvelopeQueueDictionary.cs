using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedObjects;

namespace CommSub
{
    public class EnvelopeQueueDictionary
    {
        private ConcurrentDictionary<MessageNumber, EnvelopeQueue> dictionary;

        public EnvelopeQueueDictionary()
        {
            dictionary = new ConcurrentDictionary<MessageNumber, EnvelopeQueue>(new MessageNumber.MessageNumberComparer());
        }

        public EnvelopeQueue GetByConversationId(MessageNumber label)
        {
            EnvelopeQueue queue = null;
            dictionary.TryGetValue(label, out queue);
            return queue;
        }

        public EnvelopeQueue CreateQueue(MessageNumber queueId)
        {
            EnvelopeQueue queue = null;
            dictionary.TryGetValue(queueId, out queue);

            if (queue == null)
            {
                queue = new EnvelopeQueue() { QueueId = queueId };
                dictionary.TryAdd(queueId, queue);
            }

            return queue;
        }

        public void CloseQueue(MessageNumber queueId)
        {
            EnvelopeQueue queue = null;
            dictionary.TryRemove(queueId, out queue);
        }
    }
}
