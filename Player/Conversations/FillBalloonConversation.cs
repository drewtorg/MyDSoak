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
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new[] {typeof(BalloonReply) };
            }
        }

        protected override Message CreateRequest()
        {
            Penny[] pennies = new Penny[2];
            pennies[0] = ((Player)Process).Pennies.Pop();
            pennies[1] = ((Player)Process).Pennies.Pop();

            Balloon unfilled = ((Player)Process).Balloons.Where(x => x.IsFilled == false).First();
            ((Player)Process).Balloons.Remove(unfilled);

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

            ((Player)Process).Balloons.Add(balloon.Balloon);
        }
    }
}