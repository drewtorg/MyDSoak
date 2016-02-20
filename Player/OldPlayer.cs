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
    public class OldPlayer : ISubject
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(Player));
        protected readonly UdpClient client;
        protected bool done;

        public PublicEndPoint RegistryEndPoint { get; set; }
        public IdentityInfo Identity { get; set; }
        public ProcessInfo Process { get; set; }
        public GameInfo Game { get; set; }
        public List<IObserver> Observers { get; set; }

        private List<GameInfo> potentialGames = null;

        public OldPlayer(string endpoint, IdentityInfo info)
        {
            Logger.Debug("Creating new player");

            RegistryEndPoint = new PublicEndPoint(endpoint);
            Identity = info;

            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 0);
            client = new UdpClient(localEp);

            Observers = new List<IObserver>();
        }
        
        public void Start()
        {
            Logger.Debug("Starting up Player object");
            done = false;
            SendLoginRequest();
            ReceiveData();
        }

        public void Stop()
        {
            Logger.Debug("Stopping Player object");
            done = true;
            foreach (var observer in Observers)
                observer.Remove(this);
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
        }

        public void SendLogoutRequest()
        {
            LogoutRequest msg = new LogoutRequest();

            byte[] bytes = msg.Encode();

            client.Send(bytes, bytes.Length, RegistryEndPoint.IPEndPoint);

            Logger.Debug("Sent LogoutRequest: " + Encoding.ASCII.GetString(bytes));
        }

        public void ReceiveData()
        {
            try
            {
                client.BeginReceive(new AsyncCallback(ReceiveCallback), null);
            }
            catch (SocketException e)
            {
                Logger.Debug(e.ToString());
            }
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

                    Notify(); //Update display after receiving any message
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
            Process.Status = ProcessInfo.StatusCode.Registered;

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
            Process.Status = ProcessInfo.StatusCode.JoiningGame;

            potentialGames = reply.GameInfo.ToList();

            SendJoinGameRequest();
        }

        public void ReceiveMessage(JoinGameReply reply)
        {
            Logger.Debug("Was JoinGameReply");

            if (reply.Success)
            {
                Process.Status = ProcessInfo.StatusCode.JoinedGame;
                Game = potentialGames[0];
                Process.LifePoints = (short)reply.InitialLifePoints;
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
            Process.Status = ProcessInfo.StatusCode.Terminating;
            Stop();
        }

        public void Subscribe(IObserver observer)
        {
            if (observer != null && !Observers.Contains(observer))
            {
                if (observer is PlayerForm)
                    ((PlayerForm)observer).Player = this;
                Observers.Add(observer);
            }
        }

        public void Unsubscribe(IObserver observer)
        {
            if (observer != null && Observers.Contains(observer))
            {
                Observers.Remove(observer);
                observer.Remove(this);
            }
        }

        public void Notify()
        {
            foreach (var observer in Observers)
                observer.Update(this);
        }
    }
}
