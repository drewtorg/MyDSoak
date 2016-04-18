using System;
using CommSub.Conversations.ResponderConversations;
using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using SharedObjects;

namespace Player.Conversations
{
    public class AuctionConversation : RequestReply
    {
        private Penny[] pennies = null;

        protected override Type[] AllowedTypes
        {
            get
            {
                return new Type[] { typeof(AuctionAnnouncement) };
            }
        }

        protected override Message CreateReply()
        {
            AuctionAnnouncement auction = Request as AuctionAnnouncement;

            Bid bid = null;

            if (auction.MinimumBid <= ((Player)Process).Pennies.Count)
            {
                pennies = new Penny[auction.MinimumBid];
                ((Player)Process).Pennies.TryPopRange(pennies);
                bid =  new Bid()
                {
                    Pennies = pennies,
                    Success = true
                };
            }
            else
                bid = new Bid() { Success = false };

            return bid;
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() &&
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }
    }
}