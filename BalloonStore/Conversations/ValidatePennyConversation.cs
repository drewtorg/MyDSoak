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
        BuyBalloonConversation parent = null;
        Penny penny = null;

        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new Type[] { typeof(PennyValidation) };
            }
        }

        protected override Message CreateRequest()
        {
            return new PennyValidation()
            {
                MarkAsUsedIfValid = true,
                Pennies = new Penny[] { penny }
            };
        }

        protected override void ProcessReply(Reply reply)
        {
            if (reply.Success)
                parent.ValidatedByPennyBank = true;
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() 
                && Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }
    }
}
