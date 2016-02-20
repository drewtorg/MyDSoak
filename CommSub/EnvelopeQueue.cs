using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedObjects;
using System.Threading;

namespace CommSub
{
    public class EnvelopeQueue
    {
        public int Count { get { return queue.Count; } }
        public MessageNumber QueueId { get; set; }

        private ConcurrentQueue<Envelope> queue;

        public EnvelopeQueue()
        {
            queue = new ConcurrentQueue<Envelope>();
        }

        public void Enqueue(Envelope envelope) => queue.Enqueue(envelope);

        public Envelope Dequeue(int timeout)
        {
            Envelope result = null;
            bool successful = queue.TryDequeue(out result);

            if(!successful)
            {
                Thread.Sleep(timeout);
                queue.TryDequeue(out result);
            }
            return result;
        }
    }
}
