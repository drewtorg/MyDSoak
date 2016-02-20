using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utils;

using SharedObjects;

using Messages.RequestMessages;

namespace CommSub.Conversations
{
    public abstract class Conversation : BackgroundThread
    {
        public int Tries { get; set; }
        public int Timeout { get; set; }
        // public PublicEndPoint Initiator { get; set; }
        // public PublicEndPoint Responder { get; set; }
        public CommSubsystem CommSubsystem { get; set; }
        public MessageNumber Id { get; set; }
    }
}
