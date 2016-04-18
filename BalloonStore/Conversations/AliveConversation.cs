using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations.ResponderConversations;

using Messages.ReplyMessages;
using Messages.RequestMessages;

using log4net;
using Messages;
using SharedObjects;

namespace BalloonStore.Conversations
{
    public class AliveConversation : RequestReply
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                return new Type[] { typeof(AliveRequest) };
            }
        }

        protected override Message CreateReply()
        {
            return new Reply()
            {
                Success = true,
                Note = "I'm alive!"
            };
        }
    }
}
