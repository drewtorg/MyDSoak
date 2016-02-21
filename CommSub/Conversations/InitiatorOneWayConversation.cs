using SharedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;

namespace CommSub.Conversations
{
    public abstract class InitiatorOneWayConversation : Conversation
    {
        public List<ProcessInfo> Receivers { get; set; }

        protected override void Process(object state)
        {
            Initialize();

            foreach(ProcessInfo process in Receivers)
            {
                //create the message for each receiver
                Envelope message = new Envelope()
                {
                    Message = CreateMessage(),
                    Ep = process.EndPoint
                };

                //send of the message
                CommSubsystem.Communicator.Send(message);
            }

            Stop();
        }

        protected abstract Message CreateMessage();
    }
}
