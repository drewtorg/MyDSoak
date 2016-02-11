using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversation;

namespace CommSub
{
    public class CommSubsystem
    {
        public Communicator Communicator { get; set; }
        public EnvelopeQueueDictionary EnvelopeQueueDictionary { get; set; }
        public Dispatcher Dispatcher { get; set; }
        public Doer Doer { get; set; }
        public ProcessAddressBook ProcessAddressBook { get; set; }
        public ConversationFactory ConversationFactory { get; set; }
    }
}
