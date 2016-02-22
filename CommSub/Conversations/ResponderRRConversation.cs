using Messages.ReplyMessages;
using SharedObjects;
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
                Envelope reply = new Envelope()
                {
                    Message = CreateReply(),
                    Ep = ReceivedEnvelope.Ep
                };
                // the msgNr is the next one for this process
                // the convId is the same as the requester's convId
                reply.Message.SetMessageAndConversationNumbers(MessageNumber.Create(), ReceivedEnvelope.Message.ConvId);

                CommSubsystem.Communicator.Send(reply);
                Successful = true;
            }

            if (!Successful)
                ProcessFailure();
            
            Stop();
        }

        protected abstract Reply CreateReply();
        protected abstract void ProcessRequest();
        protected abstract bool ValidateProcessState();
        protected abstract void ProcessFailure();
    }
}
