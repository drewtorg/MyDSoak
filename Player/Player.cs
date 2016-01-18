using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private bool done;

        public PublicEndPoint RegistryEndPoint { get; set; }
        public PublicEndPoint GameManagerEndPoint { get; set; }
        public IdentityInfo Identity { get; set; }
        public ProcessInfo Process { get; set; }
        public GameInfo Game { get; set; }
        public PlayerForm Form { get; set; }

        private List<GameInfo> potentialGames = null;

        public Player(string endpoint, IdentityInfo info)
        {
            Logger.Debug("Creating new player");

            RegistryEndPoint = new PublicEndPoint(endpoint);
            Identity = info;

            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 0);
            client = new UdpClient(localEp);
        }
        
        public void Start()
        {
            done = false;
            SendLoginRequest();
            ReceiveData();
        }

        public void Stop()
        {
            done = true;
        }

        public void SendLoginRequest()
        {
            ThreadHelper.SetText(Form, Form.StatusLabel, "Logging In");

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
                ThreadHelper.SetText(Form, Form.StatusLabel, "Joining Game");


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
            //else
            //{
            //    Thread.Sleep(500);
            //    SendGameListRequest();
            //}
        }

        public void SendLogoutRequest()
        {
            ThreadHelper.SetText(Form, Form.StatusLabel, "Logging Out");


            LogoutRequest msg = new LogoutRequest();

            byte[] bytes = msg.Encode();

            client.Send(bytes, bytes.Length, RegistryEndPoint.IPEndPoint);

            Logger.Debug("Sent LogoutRequest: " + Encoding.ASCII.GetString(bytes));
        }

        public void ReceiveData()
        {
            client.BeginReceive(new AsyncCallback(ReceiveCallback), null);
        }

        public void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);

                byte[] bytes = client.EndReceive(result, ref ep);

                if (bytes != null)
                {
                    Message message = Message.Decode(bytes);

                    Logger.Debug("Received Message: " + Encoding.ASCII.GetString(bytes));

                    ReceiveMessage((dynamic)message);
                }

                if (!done)
                    ReceiveData();
            }
            catch (Exception e)
            {
                Logger.Debug(e);
            }

        }

        public void ReceiveMessage(LoginReply reply)
        {
            Process = reply.ProcessInfo;

            ThreadHelper.SetText(Form, Form.EndpointLabel, Process.EndPoint.HostAndPort);
            ThreadHelper.SetText(Form, Form.ProcessLabel, String.Format("{0}({1})", Identity.Alias, Process.ProcessId));
            ThreadHelper.SetText(Form, Form.StatusLabel, "Logged In");

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

            ThreadHelper.SetText(Form, Form.StatusLabel, "In Game");
            
            if (reply.Success)
            {
                Game = potentialGames[0];
                Process.LifePoints = (short)reply.InitialLifePoints;
                ThreadHelper.AddListViewItem(Form, Form.ProcessListView, new System.Windows.Forms.ListViewItem(new string[]
                                                                {
                                                                    "Life Points",
                                                                    Process.LifePoints.ToString()
                                                                }));
            }
            else
            {
                potentialGames.RemoveAt(0);
                SendJoinGameRequest();
            }
        }

        public void ReceiveMessage(Reply reply)
        {
            Logger.Debug("Was LogoutReply");
            Stop();
        }
    }
}
