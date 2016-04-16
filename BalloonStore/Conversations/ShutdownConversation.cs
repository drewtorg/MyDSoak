using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ReceiverConversations;
using Messages;

namespace BalloonStore.Conversations
{
    public class ShutdownConversation : Receiver
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override void HandleRequest(Message request)
        {
            throw new NotImplementedException();
        }
    }
}
