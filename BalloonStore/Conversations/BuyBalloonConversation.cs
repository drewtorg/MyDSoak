using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ResponderConversations;
using Messages;

namespace BalloonStore.Conversations
{
    public class BuyBalloonConversation : RequestReply
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override Message CreateReply()
        {
            throw new NotImplementedException();
        }
    }
}
