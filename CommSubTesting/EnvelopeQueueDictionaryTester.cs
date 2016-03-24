using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using CommSub;
using SharedObjects;

namespace CommSubTesting
{
    [TestFixture]
    public class EnvelopeQueueDictionaryTester
    {
        //EnvelopeQueueDictionary dictionary = null;

        //[SetUp]
        //public void Setup()
        //{
        //    dictionary = new EnvelopeQueueDictionary();
        //}

        //// tests for:
        ////  create queue with null message number should only reutrn null
        ////  create queue with message number should return a queue with the same id as the message number
        ////  create queue with existing message number should return the same queue as before
        //[Test]
        //public void EnvelopeQueueDictionary_CreateQueue()
        //{
        //    MessageNumber num = new MessageNumber() { Pid = 1, Seq = 1 };

        //    EnvelopeQueue queue = null;

        //    queue = dictionary.CreateQueue(null);
        //    Assert.That(queue, Is.Null);

        //    queue = dictionary.CreateQueue(num);
        //    Assert.That(queue, Is.Not.Null);
        //    Assert.That(queue.QueueId, Is.EqualTo(num));

        //    queue.Enqueue(new Envelope());

        //    queue = dictionary.CreateQueue(num);
        //    Assert.That(queue.Count, Is.EqualTo(1));
        //}

        //// tests for:
        ////  close queue with a null message number
        ////  close queue with a message number that hasn't been created yet
        ////  close queue and finding it should be null again
        //[Test]
        //public void EnvelopeQueueDictionary_CloseQueue()
        //{
        //    MessageNumber num = new MessageNumber() { Pid = 1, Seq = 1 };

        //    try {
        //        dictionary.CloseQueue(null);
        //    }
        //    catch(Exception err)
        //    {
        //        Assert.Fail();
        //    }

        //    try
        //    {
        //        dictionary.CloseQueue(num);
        //    }
        //    catch (Exception err)
        //    {
        //        Assert.Fail();
        //    }

        //    dictionary.CreateQueue(num);
        //    dictionary.CloseQueue(num);

        //    EnvelopeQueue queue = dictionary.GetByConversationId(num);
        //    Assert.That(queue, Is.Null);
        //}

        //// tests for:
        ////  get queue with null message number
        ////  get queue with message number that hasn't been created yet
        ////  get queue with valid messsage number should return that same queue as was created
        //[Test]
        //public void EnvelopeQueueDictionary_GetConversationById()
        //{
        //    MessageNumber num = new MessageNumber() { Pid = 1, Seq = 1 };

        //    EnvelopeQueue output = dictionary.GetByConversationId(null);
        //    Assert.That(output, Is.Null);

        //    output = dictionary.GetByConversationId(num);
        //    Assert.That(output, Is.Null);

        //    EnvelopeQueue queue = dictionary.CreateQueue(num);
        //    output = dictionary.GetByConversationId(num);
        //    Assert.That(output, Is.EqualTo(queue));
        //}
    }
}
