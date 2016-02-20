using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations;
using Messages.RequestMessages;

namespace Player.Conversations
{
    public class LogoutConversation : InitiatorRRConversation
    {
        protected override Request CreateRequest()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessFailure()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessReply(Envelope envelope)
        {
            throw new NotImplementedException();
        }

        protected override bool ValidateConversationState()
        {
            throw new NotImplementedException();
        }

        protected override bool ValidateProcessState()
        {
            throw new NotImplementedException();
        }
    }
}
