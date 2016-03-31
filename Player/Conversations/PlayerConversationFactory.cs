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
        // in order from Table 1 - Protocol List
        public override void Initialize()
        {
            Add(typeof(LoginRequest), typeof(LoginConversation));
            Add(typeof(LogoutRequest), typeof(LogoutConversation));
            Add(typeof(AliveRequest), typeof(AliveConversation));
            Add(typeof(GameListRequest), typeof(GameListConversation));
            Add(typeof(JoinGameRequest), typeof(JoinGameConversation));
            Add(typeof(AllowanceDeliveryRequest), typeof(AllowanceDistributionConversation));
            Add(typeof(ReadyToStart), typeof(ReadyToStartConversation));
            Add(typeof(GameStatusNotification), typeof(GameStatusConversation));
            Add(typeof(BuyBalloonRequest), typeof(BuyBalloonConversation));
            Add(typeof(FillBalloonRequest), typeof(FillBalloonConversation));
            Add(typeof(ThrowBalloonRequest), typeof(ThrowBalloonConversation));
            Add(typeof(HitNotification), typeof(HitByBalloonConversation));
            // Add(typeof(AuctionAnnouncement), typeof(AuctionConversation));
            // Add(typeof(RaiseUmbrella), typeof(RaiseUmbrellaConversation));
            // Add(typeof(LowerUmbrella), typeof(LowerUmbrellaConversation));
            Add(typeof(LeaveGameRequest), typeof(LeaveGameConversation));
            // Add(typeof(GetKeyRequest), typeof(GetKeyConversation));
            Add(typeof(PennyValidation), typeof(PennyValidationConversation));
            Add(typeof(ShutdownRequest), typeof(ShutdownConversation));
        }
    }
}
