using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.InitiatorConversations;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace BalloonStore.Conversations
{
    public class ValidatePennyConversation : RequestReply
    {
        public BuyBalloonConversation Parent = null;
        public Penny[] Pennies= null;

        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new Type[] { typeof(Reply) };
            }
        }

        protected override Message CreateRequest()
        {
            return new PennyValidation()
            {
                MarkAsUsedIfValid = true,
                Pennies = Pennies
            };
        }

        protected override void ProcessReply(Reply reply)
        {
            Parent.ValidatedByPennyBank = reply.Success;
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() 
                && Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() 
                && Parent != null 
                && Pennies != null;
        }
    }
}
