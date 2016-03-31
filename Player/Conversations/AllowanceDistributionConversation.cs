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
            TcpListener server = new TcpListener(IPAddress.Any, req.PortNumber);
            server.Start();
            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();

            ((Player)Process).Pennies.Clear();

            for(int i = 0; i < req.NumberOfPennies; i++)
                ((Player)Process).Pennies.Push(NetworkStreamExtensions.ReadStreamMessage(stream));

            return new Reply()
            {
                Note = "Thanks for the cash!",
                Success = true
            };
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() && 
                (Process.MyProcessInfo.Status == ProcessInfo.StatusCode.JoinedGame ||  // its possible the pennies come before the JoinGameReply
                 Process.MyProcessInfo.Status == ProcessInfo.StatusCode.JoiningGame);
        }
    }
}
