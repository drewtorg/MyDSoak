using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using System.Net;
using System.Net.Sockets;

using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;

using SharedObjects;

using log4net;

namespace Player
{
    public class Player
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Player));
        private readonly UdpClient client;

        public PublicEndPoint RegistryEndPoint { get; set; }
        public PublicEndPoint GameManagerEndPoint { get; set; }
        public IdentityInfo Identity { get; set; }
        public ProcessInfo Process { get; set; }
        public GameInfo Game { get; set; }

        private List<GameInfo> potentialGames = null;

        public Player(string endpoint, IdentityInfo info)
        {
            Logger.Debug("Creating new player");

            RegistryEndPoint = new PublicEndPoint(endpoint);
            Identity = info;

            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 0);
            client = new UdpClient(localEp);
        }

        public void SendLoginRequest()
        {
            LoginRequest msg = new LoginRequest()
            {
                Identity = this.Identity,
                ProcessType = ProcessInfo.ProcessType.Player,
                ProcessLabel = "Drew Torgeson"
            };

            byte[] bytes = msg.Encode();

            client.Send(bytes, bytes.Length, RegistryEndPoint.IPEndPoint);

            Logger.Debug("Sent LoginRequest: " + Encoding.ASCII.GetString(bytes));
        }


        public void SendGameListRequest()
        {
            GameListRequest msg = new GameListRequest()
            {
                StatusFilter = (int)GameInfo.StatusCode.Available
            };

            byte[] bytes = msg.Encode();

            client.Send(bytes, bytes.Length, RegistryEndPoint.IPEndPoint);

            Logger.Debug("Sent GameListRequest: " + Encoding.ASCII.GetString(bytes));
        }

        public void SendAliveReply()
        {
            Reply reply = new Reply();
            reply.Success = true;

            byte[] bytes = reply.Encode();
            client.Send(bytes, bytes.Length, RegistryEndPoint.IPEndPoint);

            Logger.Debug("Sent Reply: " + Encoding.ASCII.GetString(bytes));
        }

        public void SendJoinGameRequest()
        {
            if (potentialGames.Count != 0)
            {

                GameInfo game = potentialGames[0];

                JoinGameRequest msg = new JoinGameRequest()
                {
                    GameId = game.GameId,
                    Player = Process
                };

                byte[] bytes = msg.Encode();

                client.Send(bytes, bytes.Length, game.GameManager.EndPoint.IPEndPoint);

                Logger.Debug("Sent JoinGameRequest: " + Encoding.ASCII.GetString(bytes));
            }
            else
            {
                SendGameListRequest();
            }
        }

        public void ReceiveMessage()
        {
            IPEndPoint ep = null;
            byte[] bytes = client.Receive(ref ep);

            Message message = Message.Decode(bytes);

            Logger.Debug("Received Message: " + Encoding.ASCII.GetString(bytes));

            ReceiveMessage((dynamic) message);

            ReceiveMessage();
        }

        public void ReceiveMessage(LoginReply reply)
        {
            Process = reply.ProcessInfo;

            Logger.Debug("Was LoginReply");

            SendGameListRequest();
        }

        public void ReceiveMessage(AliveRequest request)
        {
            Logger.Debug("Was AliveRequest");

            SendAliveReply();
        }

        public void ReceiveMessage(GameListReply reply)
        {
            Logger.Debug("Was GameListReply");

            potentialGames = reply.GameInfo.ToList();

            SendJoinGameRequest();
        }

        public void ReceiveMessage(JoinGameReply reply)
        {
            Logger.Debug("Was JoinGameReply");

            if (reply.Success)
            {
                Game = potentialGames[0];
                Process.LifePoints = (short)reply.InitialLifePoints;
            }
            else
            {
                potentialGames.RemoveAt(0);
                SendJoinGameRequest();
            }
        }
    }
}
