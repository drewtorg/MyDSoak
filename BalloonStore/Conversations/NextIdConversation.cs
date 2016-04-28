using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations.InitiatorConversations;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;
using log4net;
using Messages;

namespace BalloonStore.Conversations
{
    public class NextIdConversation : RequestReply
    {
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new Type[] { typeof(NextIdReply) };
            }
        }

        protected override Message CreateRequest()
        {
            return new NextIdRequest()
            {
                NumberOfIds = ((BalloonStore)Process).Options.NumBalloons
            };
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() && Process.MyProcessInfo.Status == ProcessInfo.StatusCode.Registered;
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() &&
                ((BalloonStore)Process).Options.NumBalloons > 0;
        }

        protected override void ProcessReply(Reply reply)
        {
            NextIdReply idReply = reply as NextIdReply;

            if (idReply.Success)
            {
                ((BalloonStore)Process).NextId = idReply.NextId;
                ((BalloonStore)Process).NumIds = idReply.NumberOfIds;
            }
        }
    }
}
