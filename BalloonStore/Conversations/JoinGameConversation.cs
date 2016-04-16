using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.InitiatorConversations;
using Messages;

namespace BalloonStore.Conversations
{
    public class JoinGameConversation : RequestReply
    {
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override Message CreateRequest()
        {
            throw new NotImplementedException();
        }
    }
}
