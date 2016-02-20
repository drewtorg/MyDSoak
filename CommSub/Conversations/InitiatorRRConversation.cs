using Messages.RequestMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommSub.Conversations
{
    public abstract class InitiatorRRConversation : Conversation
    {
        protected override void Process(object state)
        {
            Envelope envelope = new Envelope()
            {
                Message = CreateRequest()
            };

            for (int i = 0; i < Tries; i++)
            {
                CommSubsystem.Communicator.Send(envelope);
                Envelope reply = CommSubsystem.Communicator.Receive(Timeout);
                if (reply != null && ValidateConversationState() && ValidateProcessState())
                    ProcessReply(envelope);
            }

            ProcessFailure();
        }

        protected abstract bool ValidateConversationState();
        protected abstract bool ValidateProcessState();
        protected abstract Request CreateRequest();
        protected abstract void ProcessReply(Envelope envelope);
        protected abstract void ProcessFailure();
    }
}
