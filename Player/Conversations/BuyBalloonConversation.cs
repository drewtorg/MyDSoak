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
        private Penny penny = null;

        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new[] { typeof(BalloonReply) };
            }
        }

        protected override Message CreateRequest()
        {
            while(penny == null)
                ((Player)Process).Pennies.TryPop(out penny);

            return new BuyBalloonRequest()
            {
                Penny = penny
            };
        }

        protected override void ProcessReply(Reply reply)
        {
            BalloonReply balloon = reply as BalloonReply;
            if (balloon.Success)
                ((Player)Process).Balloons.Enqueue(balloon.Balloon);
            else
                ((Player)Process).Pennies.Push(penny);
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