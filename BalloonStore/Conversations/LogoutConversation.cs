using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations.InitiatorConversations;
using Messages.RequestMessages;
using SharedObjects;
using log4net;
using Messages.ReplyMessages;
using Messages;

namespace BalloonStore.Conversations
{
    public class LogoutConversation : RequestReply
    {
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new Type[] { typeof(Reply) };
            }
        }

        protected override Message CreateRequest()
        {
            return new LogoutRequest();
        }

        protected override bool IsProcessStateValid()
        {
            return Process.MyProcessInfo.Status != ProcessInfo.StatusCode.Initializing &&
                   Process.MyProcessInfo.Status != ProcessInfo.StatusCode.NotInitialized;
        }

        protected override void ProcessReply(Reply reply)
        {
            Process.BeginShutdown();
        }
    }
}
