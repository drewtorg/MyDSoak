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
            Initialize();

            Successful = false;

            if (ReceivedEnvelope != null && ValidateConversationState() && ValidateProcessState())
            {
                ProcessRequest();
                Successful = SendReply();
            }
            

            if (!Successful)
                ProcessFailure();
            
            Stop();
        }

        protected abstract bool SendReply();
        protected abstract void ProcessRequest();
        protected abstract bool ValidateProcessState();
        protected abstract void ProcessFailure();
    }
}
