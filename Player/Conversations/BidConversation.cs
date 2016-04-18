using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ReceiverConversations;
using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using SharedObjects;

namespace Player.Conversations
{
    public class BidConversation : Receiver
    {

        protected override Type[] AllowedTypes
        {
            get
            {
                return new Type[] { typeof(BidAck) };
            }
        }

        protected override void HandleRequest(Message request)
        {
            BidAck ack = request as BidAck;

            if(ack.Success && ack.Won)
                ((Player)Process).Umbrella = ack.Umbrella;
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() &&
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }
    }
}
