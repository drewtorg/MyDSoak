using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations;

namespace CommSub
{
    public class CommSubsystem
    {
        public Communicator Communicator { get; set; }
        public EnvelopeQueueDictionary EnvelopeQueueDictionary { get; set; }
        public Dispatcher Dispatcher { get; set; }
        public ProcessAddressBook ProcessAddressBook { get; set; }
        public ConversationFactory ConversationFactory { get; set; }

        public void Initialize()
        {
            Communicator = new Communicator();
            EnvelopeQueueDictionary = new EnvelopeQueueDictionary();
            Dispatcher = new Dispatcher() { CommSubsystem = this, Label = "Dispatcher" };
            ProcessAddressBook = new ProcessAddressBook();
        }

        public void Start()
        {
            Dispatcher.Start();
        }

        public void Stop()
        {
            Dispatcher.Stop();
        }
    }
}
