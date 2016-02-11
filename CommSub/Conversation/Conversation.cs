using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedObjects;
using Messages.RequestMessages;

namespace CommSub.Conversation
{
    public abstract class Conversation
    {
        public int Tries { get; set; }
        public int Timeout { get; set; }
        public PublicEndPoint Initiator { get; set; }
        public PublicEndPoint Responder { get; set; }
        public Communicator Communicator { get; set; }
        public MessageNumber Id { get; set; }

        public abstract bool Execute();
    }
}
