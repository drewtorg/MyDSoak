using CommSub.Conversations.InitiatorConversations;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System;
using SharedObjects;
using System.Linq;

namespace Player.Conversations
{
    public class FillBalloonConversation : RequestReply
    {
        Penny[] pennies = new Penny[2];

        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new[] {typeof(BalloonReply) };
            }
        }

        protected override Message CreateRequest()
        {
            ((Player)Process).Pennies.TryPopRange(pennies);

            Balloon unfilled = null;
            ((Player)Process).Balloons.TryDequeue(out unfilled);

            while (unfilled.IsFilled)
            {
                ((Player)Process).Balloons.Enqueue(unfilled);
                ((Player)Process).Balloons.TryDequeue(out unfilled);
            }

            return new FillBalloonRequest()
            {
                Balloon = unfilled,
                Pennies = pennies
            };
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() &&
                ((Player)Process).Balloons.Where(x => x.IsFilled == false).Count() > 0 &&
                ((Player)Process).WaterSources.Count > 0;
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() && 
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }

        protected override void ProcessReply(Reply reply)
        {
            BalloonReply balloon = reply as BalloonReply;

            if(reply.Success)
                ((Player)Process).Balloons.Enqueue(balloon.Balloon);
            else
            {
                ((Player)Process).Pennies.Push(pennies[0]);
                ((Player)Process).Pennies.Push(pennies[1]);
            }
        }
    }
}