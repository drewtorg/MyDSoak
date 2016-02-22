using System;
using NUnit.Framework;

using CommSub;

namespace CommSubTesting
{
    [TestFixture]
    public class EnvelopeQueueTester
    {
        EnvelopeQueue queue = null;
        Envelope e = null;

        [SetUp]
        public void Setup()
        {
            queue = new EnvelopeQueue();
            e = new Envelope();
        }

        // tests for:
        //  multiple enqueues should not be counted
        //  enqueue of null should not enqueue anything
        [Test]
        public void EnvelopeQueue_Enqueue()
        {
            Assert.That(queue.Count, Is.EqualTo(0));

            queue.Enqueue(e);
            queue.Enqueue(e);
            queue.Enqueue(null);

            Assert.That(queue.Count, Is.EqualTo(1));
        }

        // tests for:
        //  dequeue empty queue should be null
        //  dequeue should return first enqueued object
        [Test]
        public void EnvelopQueue_Dequeue()
        {
            EnvelopeQueue queue = new EnvelopeQueue();
            Envelope e = new Envelope();
            Envelope e2 = new Envelope();

            Envelope output = queue.Dequeue(1);

            Assert.That(output, Is.Null);
            Assert.That(queue.Count, Is.EqualTo(0));

            queue.Enqueue(e);
            queue.Enqueue(e2);

            output = queue.Dequeue(10);
            Assert.That(output, Is.EqualTo(e));
        }
    }
}
