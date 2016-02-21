using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommSub.Conversations
{
    public abstract class ResponderOneWayConversation : Conversation
    {
        protected override void Process(object state)
        {
            Initialize();

            if (ReceivedEnvelope != null && ValidateProcessState())
            {
                ProcessMessage();
            }

            Stop();
        }

        protected abstract void ProcessMessage();
        protected abstract bool ValidateProcessState();
    }
}
