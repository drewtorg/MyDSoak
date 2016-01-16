using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;

using SharedObjects;

namespace Player
{
    public class Player
    {
        private readonly UdpClient client;

        public PublicEndPoint EndPoint { get; set; }
        public IdentityInfo IdentityI { get; set; }

        public Player(string endpoint, IdentityInfo info)
        {
            EndPoint = new PublicEndPoint(endpoint);
            IdentityI = info;

            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 0);
            client = new UdpClient(localEp);
        }

        public void SendLoginRequest()
        {


            LoginRequest msg = new LoginRequest()
            {
                Identity = IdentityI,
                ProcessType = ProcessInfo.ProcessType.Player,
                ProcessLabel = "Dat Label"
            };

            byte[] bytes = msg.Encode();

            client.Send(bytes, bytes.Length, EndPoint.IPEndPoint);

            IPEndPoint ep = null;
            byte[] replyBytes = client.Receive(ref ep);

            LoginReply reply = Message.Decode(replyBytes) as LoginReply;
        }
    }
}
