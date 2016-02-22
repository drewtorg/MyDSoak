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

        private AutoResetEvent waitEvent;

        public EnvelopeQueue()
        {
            queue = new ConcurrentQueue<Envelope>();
            waitEvent = new AutoResetEvent(false);
        }

        public void Enqueue(Envelope envelope)
        {
            if (envelope != null && !queue.Contains(envelope))
            {
                queue.Enqueue(envelope);
                waitEvent.Set();
            }
        }

        public Envelope Dequeue(int timeout)
        {
            Envelope result = null;

            if (waitEvent.WaitOne(timeout))
                queue.TryDequeue(out result);
            
            return result;
        }
    }
}
