
using System;
using CommSub.Conversations.ResponderConversations;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;

namespace Player.Conversations
{
    public class LowerUmbrellaConversation : RequestReply
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                return new Type[] { typeof(UmbrellaLoweredNotification) };
            }
        }

        protected override Message CreateReply()
        {
            ((Player)Process).Umbrella = null;
            ((Player)Process).UmbrellaRaised = false;
            return new Reply()
            {
                Success = true
            };
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() &&
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() && ((Player)Process).Umbrella != null;
        }
    }
}