using CommSub.Conversations.InitiatorConversations;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System;
using SharedObjects;
using System.Linq;

namespace Player.Conversations
{
    public class ThrowBalloonConversation : RequestReply
    {
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new[] { typeof(Reply) };
            }
        }

        protected override Message CreateRequest()
        {
            Balloon filled = ((Player)Process).Balloons.Where(x => x.IsFilled).First();
            ((Player)Process).Balloons.Remove(filled);

            ToProcessId = ((Player)Process).Game.GameManagerId;

            return new ThrowBalloonRequest()
            {
                TargetPlayerId = ((Player)Process).OtherPlayers.Where(x => x.LifePoints > 0).First().ProcessId,
                Balloon = filled
            };
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() &&
                ((Player)Process).Balloons.Where(x => x.IsFilled).Count() > 0 &&
                ((Player)Process).OtherPlayers.Where(x => x.LifePoints > 0).Count() > 0;
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() &&
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }
    }
}