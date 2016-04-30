using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ResponderConversations;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;
using System.Net;
using System.Net.Sockets;
using CommSub;
using System.Threading;

namespace Player.Conversations
{
    public class AllowanceDistributionConversation : RequestReply
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                return new[] { typeof(AllowanceDeliveryRequest) };
            }
        }

        protected override Message CreateReply()
        {
            AllowanceDeliveryRequest req = Request as AllowanceDeliveryRequest;
            TcpClient client = new TcpClient(((Player)Process).PennyBankEndPoint.Host, req.PortNumber);
            NetworkStream stream = client.GetStream();
            stream.ReadTimeout = 100;

            ThreadPool.QueueUserWorkItem(GetPennies, stream);

            return new Reply()
            {
                Note = "Thanks for the cash!",
                Success = true
            };
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() &&
                (Process.MyProcessInfo.Status == ProcessInfo.StatusCode.JoinedGame ||  // its possible the pennies come before the JoinGameReply or after the game starts
                 Process.MyProcessInfo.Status == ProcessInfo.StatusCode.JoiningGame ||
                 Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame);
        }

        private void GetPennies(object myStream)
        {
            AllowanceDeliveryRequest req = Request as AllowanceDeliveryRequest;
            NetworkStream stream = myStream as NetworkStream;

            ((Player)Process).Pennies.Clear();

            for (int i = 0; i < req.NumberOfPennies; i++)
                ((Player)Process).Pennies.Push(NetworkStreamExtensions.ReadStreamMessage(stream));
        }
    }
}
