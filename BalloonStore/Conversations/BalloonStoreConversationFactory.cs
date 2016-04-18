using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;
using Messages.RequestMessages;
using Messages.ReplyMessages;

namespace BalloonStore.Conversations
{
    public class BalloonStoreConversationFactory : ConversationFactory
    {
        public override void Initialize()
        {
            Add(typeof(StartGame), typeof(StartGameConversation));
            Add(typeof(GameStatusNotification), typeof(GameStatusConversation));
            Add(typeof(BuyBalloonRequest), typeof(BuyBalloonConversation));
            Add(typeof(ShutdownRequest), typeof(ShutdownConversation));
        }
    }
}
