using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using CommSub;
using SharedObjects;
using Messages;

namespace CommSubTesting
{
    [TestFixture]
    public class EnvelopeTester
    {
        // tests for:
        //  simple construction, just to make sure nothing weird happens
        [Test]
        public void Envelope_Constructor()
        {
            PublicEndPoint ep = new PublicEndPoint("127.0.0.1:1000");
            Message message = new Message();
            Envelope e = new Envelope() { Ep = ep, Message = message };

            Assert.That(e.Message, Is.EqualTo(message));
            Assert.That(e.Ep, Is.EqualTo(ep));
        }
    }
}
