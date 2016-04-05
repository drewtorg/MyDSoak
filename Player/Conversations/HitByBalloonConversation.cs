using CommSub.Conversations.ResponderConversations;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System;
using SharedObjects;
using System.Linq;

namespace Player.Conversations
{
    public class HitByBalloonConversation : RequestReply
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                return new[] { typeof(HitNotification) };
            }
        }

        protected override Message CreateReply()
        {
            ((Player)Process).GameData.ChangeLifePoints(-1);
            return new Reply()
            {
                Note = "Ouch...",
                Success = true
            };
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() && 
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }
    }
}