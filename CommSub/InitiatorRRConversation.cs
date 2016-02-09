using Messages.RequestMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommSub
{
    public abstract class InitiatorRRConversation : Conversation
    {
        public override bool Execute()
        {
            Envelope envelope = new Envelope()
            {
                Message = CreateRequest(),
                Ep = Responder
            };

            for (int i = 0; i < Tries; i++)
            {
                Communicator.Send(envelope);
                Envelope reply = Communicator.Receive(Timeout);
                if (reply != null && ValidateConversationState() && ValidateProcessState())
                    return ProcessReply(envelope);
            }

            return ProcessFailure();
        }

        protected abstract bool ValidateConversationState();
        protected abstract bool ValidateProcessState();
        protected abstract Request CreateRequest();
        protected abstract bool ProcessReply(Envelope envelope);
        protected abstract bool ProcessFailure();
    }
}
