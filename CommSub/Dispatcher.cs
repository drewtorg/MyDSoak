using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommSub.Conversations;

using Utils;

namespace CommSub
{
    public class Dispatcher : BackgroundThread
    {
        private const int TIMEOUT = 1000;

        public CommSubsystem CommSubsystem { get; set; }

        protected override void Process(object state)
        {
            while(KeepGoing)
            {
                Envelope envelope = CommSubsystem.Communicator.Receive(TIMEOUT);
                EnvelopeQueue queue = CommSubsystem.EnvelopeQueueDictionary.GetByConversationId(envelope.Message.ConvId);

                //when a queue is null, it's time to create a new conversation
                if (queue == null)
                {
                    if(CommSubsystem.ConversationFactory.MessageCanStartConversation(envelope.Message.GetType()))
                    {
                        Conversation conversation = CommSubsystem.ConversationFactory.CreateFromMessageType(envelope.Message.GetType());

                        conversation.Start();
                    }
                    else
                    {
                        //this message can't start a conversation and can't be replied too. It's been sent to the wrong place.
                    }
                }
                else
                {
                    queue.Enqueue(envelope);
                }

                Thread.Sleep(0);
            }
        }
    }
}
