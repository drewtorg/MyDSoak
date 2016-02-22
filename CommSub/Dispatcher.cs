using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommSub.Conversations;

using Utils;
using log4net;

namespace CommSub
{
    public class Dispatcher : BackgroundThread
    {
        private const int TIMEOUT = 1000;
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(Dispatcher));

        public CommSubsystem CommSubsystem { get; set; }

        protected override void Process(object state)
        {
            while(KeepGoing)
            {
                Logger.Debug("Top of Processing Loop");

                Envelope envelope = CommSubsystem.Communicator.Receive(TIMEOUT);
                EnvelopeQueue queue = null;
                if (envelope != null)
                {
                    queue = CommSubsystem.EnvelopeQueueDictionary.GetByConversationId(envelope.Message.ConvId);

                    //when a queue is null, it's time to create a new conversation
                    if (queue == null)
                    {
                        if (CommSubsystem.ConversationFactory.MessageCanStartConversation(envelope.Message.GetType()))
                        {
                            Conversation conversation = CommSubsystem.ConversationFactory.CreateFromMessageType(envelope.Message.GetType());
                            conversation.ReceivedEnvelope = envelope;
                            conversation.CommSubsystem = CommSubsystem;

                            Logger.Debug("Starting a new ResponderConversation");

                            conversation.Start();
                        }
                        else
                        {
                            //this message can't start a conversation and can't be replied too. It's been sent to the wrong place.
                            Logger.Warn("Received foreign Message");
                        }
                    }
                    else
                    {
                        queue.Enqueue(envelope);

                        Logger.Debug("Put Message in Queue " + queue.QueueId.ToString());
                    }
                }

                Thread.Sleep(0);
            }
        }
    }
}
