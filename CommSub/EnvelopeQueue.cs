using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedObjects;

namespace CommSub
{
    public class EnvelopeQueue
    {
        public int Count { get { return queue.Count; } }
        public MessageNumber QueueId { get; set; }

        private ConcurrentQueue<Envelope> queue;

        public void Enqueue(Envelope envelope) => queue.Enqueue(envelope);
        public Envelope Dequeue()
        {
            Envelope result = null;
            queue.TryDequeue(out result);
            return result;
        }
    }
}
