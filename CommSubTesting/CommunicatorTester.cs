using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using CommSub;
using Player.Conversations;
using Messages;
using System.Net.Sockets;
using SharedObjects;
using System.Net;

namespace CommSubTesting
{
    [TestFixture]
    public class CommunicatorTester
    {
        CommSubsystem commSub = null;

        [TestFixtureSetUp]
        public void SetUp()
        {
            commSub = new CommSubsystem(new PlayerConversationFactory());
            commSub.Initialize();
            commSub.Communicator = new TestCommunicator();
        }

        // tests for:
        //  send and receive a valid envelope
        [Test]
        public void Communicator_SendValid()
        {
            Message message = new Message();
            message.InitMessageAndConversationNumbers();

            // I'm surprised this actually works
            

            Envelope envelope = new Envelope()
            {
                Ep = ((TestCommunicator)commSub.Communicator).Ep,
                Message = message
            };
            commSub.Communicator.Send(envelope);
            Envelope output = commSub.Communicator.Receive(1000);
            Assert.That(output.Ep, Is.EqualTo(envelope.Ep));
            Assert.That(output.Message.ConvId, Is.EqualTo(envelope.Message.ConvId));
            Assert.That(output.Message.MsgId, Is.EqualTo(envelope.Message.MsgId));
        }

        // tests for:
        //  receive and nothing has been sent
        //  send and receive invalid envelopes
        [Test]
        public void Communicator_SendInvalid()
        {
            Envelope output = commSub.Communicator.Receive(10);
            Assert.That(output, Is.Null);

            commSub.Communicator.Send(null);
            output = commSub.Communicator.Receive(100);
            Assert.That(output, Is.Null);

            Envelope bogus = new Envelope();
            commSub.Communicator.Send(bogus);
            output = commSub.Communicator.Receive(100);
            Assert.That(output, Is.Null);

            bogus = new Envelope() { Message = new Message() };
            commSub.Communicator.Send(null);
            output = commSub.Communicator.Receive(100);
            Assert.That(output, Is.Null);

            bogus = new Envelope() { Ep = new PublicEndPoint("127.0.0.1:1000") };
            commSub.Communicator.Send(null);
            output = commSub.Communicator.Receive(100);
            Assert.That(output, Is.Null);
        }
    }

    public class TestCommunicator : Communicator
    {
        public PublicEndPoint Ep
        {
            get
            {
                int communicatorPort = ((IPEndPoint)client.Client.LocalEndPoint).Port;
                return new PublicEndPoint()
                {
                    Host = "127.0.0.1",
                    Port = communicatorPort
                };
            }
        }
    }
}
