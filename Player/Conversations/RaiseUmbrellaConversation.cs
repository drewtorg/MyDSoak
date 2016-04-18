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

namespace Player.Conversations
{
    public class RaiseUmbrellaConversation : RequestReply
    {
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new Type[] { typeof(Reply) };
            }
        }

        protected override Message CreateRequest()
        {
            return new RaiseUmbrella()
            {
                Umbrella = ((Player)Process).Umbrella
            };
        }

        protected override void ProcessReply(Reply reply)
        {
            if (reply.Success)
                ((Player)Process).UmbrellaRaised = true;
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() &&
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() 
                && ((Player)Process).Umbrella != null
                && ((Player)Process).UmbrellaRaised == false;
        }
    }
}
