using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedObjects;
using Messages.RequestMessages;

namespace CommSub
{
    public abstract class Conversation
    {
        public int Tries { get; set; }
        public int Timeout { get; set; }
        public PublicEndPoint Initiator { get; set; }
        public PublicEndPoint Responder { get; set; }
        public Communicator Communicator { get; set; }

        public abstract bool Execute();
    }
}
