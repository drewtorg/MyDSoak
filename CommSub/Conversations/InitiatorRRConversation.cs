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
            bool successful = false;

            Envelope envelope = new Envelope()
            {
                Message = CreateRequest()
            };

            for (int i = 0; i < Tries && !successful; i++)
            {
                CommSubsystem.Communicator.Send(envelope);
                Envelope reply = CommSubsystem.Communicator.Receive(Timeout);
                if (reply != null && ValidateConversationState() && ValidateProcessState())
                {
                    ProcessReply(envelope);
                    successful = true;
                }
            }

            if(!successful)
                ProcessFailure();
        }

        //TODO: review the rules for validation of conversation state
        protected bool ValidateConversationState()
        {
            return true;
        }

        //TODO: review the rules for validation of conversation state
        protected bool ValidateProcessState()
        {
            return true;
        }
        protected abstract Request CreateRequest();
        protected abstract void ProcessReply(Envelope envelope);
        protected abstract void ProcessFailure();
    }
}
