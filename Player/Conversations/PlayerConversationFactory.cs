using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;

using Messages.RequestMessages;
using Messages.ReplyMessages;


namespace Player.Conversations
{
    public class PlayerConversationFactory : ConversationFactory
    {
        public override void Initialize()
        {
            Add(typeof(AliveRequest), typeof(AliveConversation));
            Add(typeof(AllowanceDeliveryRequest), typeof(AllowanceDistributionConversation));
            Add(typeof(ReadyToStart), typeof(ReadyToStartConversation));
            Add(typeof(StartGame), typeof(StartGameConversation));
            Add(typeof(GameStatusNotification), typeof(GameStatusConversation));
            Add(typeof(HitNotification), typeof(HitByBalloonConversation));
            Add(typeof(AuctionAnnouncement), typeof(AuctionConversation));
            Add(typeof(BidAck), typeof(BidConversation));
            Add(typeof(UmbrellaLoweredNotification), typeof(LowerUmbrellaConversation));
            Add(typeof(ShutdownRequest), typeof(ShutdownConversation));
            Add(typeof(ExitGameRequest), typeof(ExitGameConversation));
        }
    }
}
