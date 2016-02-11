using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utils;

namespace CommSub
{
    public class Listener : BackgroundThread
    {
        public CommSubsystem CommSubsystem { get; set; }

        protected override void Process(object state)
        {
            while(keepGoing)
            {
                Envelope envelope = CommSubsystem.Communicator.Receive(1000);

                if (envelope != null)
                    CommSubsystem.EnvelopeQueueDictionary.GetByConversationId(envelope.Message.ConvId).Enqueue(envelope); // maybe???
            }
        }
    }
}
