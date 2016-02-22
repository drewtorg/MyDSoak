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

        public EnvelopeQueue GetByConversationId(MessageNumber convId)
        {
            EnvelopeQueue queue = null;
            if(convId != null)
                dictionary.TryGetValue(convId, out queue);
            return queue;
        }

        public EnvelopeQueue CreateQueue(MessageNumber queueId)
        {
            EnvelopeQueue queue = null;

            if (queueId != null)
            {
                dictionary.TryGetValue(queueId, out queue);

                if (queue == null)
                {
                    queue = new EnvelopeQueue() { QueueId = queueId };
                    dictionary.TryAdd(queueId, queue);
                }
            }

            return queue;
        }

        public void CloseQueue(MessageNumber queueId)
        {
            if (queueId != null)
            {
                EnvelopeQueue queue = null;
                dictionary.TryRemove(queueId, out queue);
            }
        }
    }
}
