using CommSub.Conversations.InitiatorConversations;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System;
using SharedObjects;
using System.Linq;

namespace Player.Conversations
{
    public class BuyBalloonConversation : RequestReply
    {
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new[] { typeof(BalloonReply) };
            }
        }

        protected override Message CreateRequest()
        {
            return new BuyBalloonRequest()
            {
                Penny = ((Player)Process).Pennies.Pop()
            };
        }

        protected override void ProcessReply(Reply reply)
        {
            BalloonReply balloon = reply as BalloonReply;
            if (balloon.Success)
                ((Player)Process).Balloons.Add(balloon.Balloon);
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() &&
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() &&
                ((Player)Process).Pennies.Count > 0 &&
                ((Player)Process).BalloonStores.Count > 0;
        }
    }
}