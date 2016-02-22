using Messages.RequestMessages;
using SharedObjects;
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
            //Initialize all variables needed to run a specific conversation
            Initialize();

            Successful = false;

            // assure the process is in the correct state to send this request
            if (ValidateProcessState())
            {

                //setup the envelope
                Envelope request = new Envelope()
                {
                    Message = CreateRequest(),
                    Ep = SendTo
                };

                //make a new queue for this conversation
                EnvelopeQueue queue = CommSubsystem.EnvelopeQueueDictionary.CreateQueue(request.Message.ConvId);

                for (int i = 0; i < Tries && !Successful; i++)
                {
                    //send out the envelope
                    CommSubsystem.Communicator.Send(request);

                    //see if there is a reply in the queue
                    Envelope reply = queue.Dequeue(Timeout);

                    if (reply != null && ValidateConversationState(request.Message.ConvId, reply.Message.ConvId))
                    {
                        Successful = ProcessReply(reply);
                    }
                }

                //After processing, close the queue that was created
                CommSubsystem.EnvelopeQueueDictionary.CloseQueue(request.Message.ConvId);
            }

            if (!Successful)
                ProcessFailure();
            
            Stop();
        }



        protected abstract bool ValidateProcessState();
        protected abstract void ProcessFailure();
        protected abstract Request CreateRequest();
        protected abstract bool ProcessReply(Envelope envelope);

        protected bool ValidateConversationState(MessageNumber requestConvId, MessageNumber replyConvId)
        {
            return requestConvId == replyConvId;
        }
    }
}
