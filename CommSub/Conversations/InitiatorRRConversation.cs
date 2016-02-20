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

            //setup the envelope
            Envelope request = new Envelope()
            {
                Message = CreateRequest(),
                Ep = SendTo
            };

           //make a new queue for this conversation
            EnvelopeQueue queue = CommSubsystem.EnvelopeQueueDictionary.CreateQueue(request.Message.ConvId);
            
            for (int i = 0; i < Tries && !successful; i++)
            {
                //send out the envelope
                CommSubsystem.Communicator.Send(request);

                //see if there is a reply in the queue
                Envelope reply = queue.Dequeue(Timeout);

                if (reply != null && ValidateConversationState() && ValidateProcessState())
                {
                    ProcessReply(reply);
                    successful = true;
                }
            }

            if (!successful)
                ProcessFailure();

            // is this necessary?
            Stop();
        }



        protected abstract bool ValidateProcessState();
        protected abstract void ProcessFailure();
        protected abstract Request CreateRequest();
        protected abstract void ProcessReply(Envelope envelope);
    }
}
