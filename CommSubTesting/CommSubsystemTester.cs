using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using CommSub;

using Player.Conversations;

namespace CommSubTesting
{
    [TestFixture]
    public class CommSubsystemTester
    {
        // tests for:
        //  initialize and assure that everything in the system has been initialized
        //  start the system starts the dispatcher
        //  stop the system stops the dispatcher
        [Test]
        public void CommSubsystem_TestEverything()
        {
            //CommSubsystem commSub = new CommSubsystem(new PlayerConversationFactory());
            //commSub.Initialize();

            //Assert.That(commSub.Communicator, Is.Not.Null);
            //Assert.That(commSub.Dispatcher, Is.Not.Null);
            //Assert.That(commSub.EnvelopeQueueDictionary, Is.Not.Null);
            //Assert.That(commSub.ProcessAddressBook, Is.Not.Null);

            //commSub.Start();

            //StringAssert.AreEqualIgnoringCase(commSub.Dispatcher.Status, "Running");

            //commSub.Stop();

            //StringAssert.AreEqualIgnoringCase(commSub.Dispatcher.Status, "Not running");
        }
    }
}
