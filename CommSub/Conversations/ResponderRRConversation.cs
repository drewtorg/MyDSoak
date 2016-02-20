using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommSub.Conversations
{
    public abstract class ResponderRRConversation : Conversation
    {
        protected override void Process(object state)
        {
            bool successful = false;

            if (ReceivedEnvelope != null && ValidateConversationState() && ValidateProcessState())
            {
                ProcessRequest();
                SendReply();
                successful = true;
            }
            

            if (!successful)
                ProcessFailure();

            // is this necessary?
            Stop();
        }

        protected abstract void SendReply();
        protected abstract void ProcessRequest();
        protected abstract bool ValidateProcessState();
        protected abstract void ProcessFailure();
    }
}
