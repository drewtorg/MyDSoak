using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommSub;
using CommSub.Conversations.InitiatorConversations;

using Utils;

using SharedObjects;

using Player.Conversations;
using log4net;
using Player.States;

namespace Player
{
    public class Player : CommProcess
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(Player));

        public GameInfo Game { get; set; }
        public IdentityInfo Identity { get; set; }
        public List<GameInfo> PotentialGames { get; set; }
        public GameProcessData GameData { get; set; }
        public PublicKey PennyBankPublicKey { get; set; }
        public List<GameProcessData> WaterSources { get; set; }
        public List<GameProcessData> BalloonStores { get; set; }
        public List<GameProcessData> UmbrellaSuppliers { get; set; }
        public List<GameProcessData> OtherPlayers { get; set; }
        public Stack<Penny> Pennies { get; set; }
        public List<Balloon> Balloons { get; set; }

        public Player()
        {
            Label = "Player";
            MyProcessInfo = new ProcessInfo()
            {
                Status = ProcessInfo.StatusCode.NotInitialized,
                Label = Label,
                Type = ProcessInfo.ProcessType.Player
            };
            MyProcessInfo.Status = ProcessInfo.StatusCode.NotInitialized;
            CleanupSession();
        }

        public override void Start()
        {
            MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            RegistryEndPoint = new PublicEndPoint(Options.Registry);
            Identity = new IdentityInfo()
            {
                Alias = Options.Alias,
                ANumber = Options.ANumber,
                FirstName = Options.FirstName,
                LastName = Options.LastName
            };
            SetupCommSubsystem(new PlayerConversationFactory()
            {
                DefaultMaxRetries = Options.Retries,
                DefaultTimeout = Options.Timeout,
                Process = this
            });
            base.Start();
        }

        protected override void Process(object state)
        {
            while (Status == "Running")
            {
                RequestReply conv = null;

                switch (MyProcessInfo.Status)
                {
                    case ProcessInfo.StatusCode.Initializing:
                        conv = CommSubsystem.ConversationFactory.CreateFromConversationType<LoginConversation>();
                        conv.TargetEndPoint = RegistryEndPoint;
                        break;
                    case ProcessInfo.StatusCode.Registered:
                        conv = CommSubsystem.ConversationFactory.CreateFromConversationType<GameListConversation>();
                        conv.TargetEndPoint = RegistryEndPoint;
                        break;
                    case ProcessInfo.StatusCode.JoiningGame:
                        conv = CommSubsystem.ConversationFactory.CreateFromConversationType<JoinGameConversation>();
                        conv.ToProcessId = PotentialGames[0].GameManagerId;
                        break;
                    case ProcessInfo.StatusCode.PlayingGame:
                        conv = Play();
                        break;
                    case ProcessInfo.StatusCode.Won:
                        EndGame();
                        break;
                    case ProcessInfo.StatusCode.Tied:
                        EndGame();
                        break;
                    case ProcessInfo.StatusCode.Lost:
                        EndGame();
                        break;
                    case ProcessInfo.StatusCode.LeavingGame:
                        EndGame();
                        break;
                    case ProcessInfo.StatusCode.Terminating:
                        Stop();
                        break;
                }

                if (conv != null)
                {
                    conv.Launch();

                    while (!conv.Done)
                        Thread.Sleep(1000);
                }
            }
        }

        //public bool CanBuyBalloon()
        //{
        //    return Pennies.Count > 1 &&
        //        BalloonStores.Count > 0;
        //}

        //public bool CanFillBalloon()
        //{
        //    return Pennies.Count > 2 &&
        //        WaterSources.Count > 0;
        //}

        // This is the method that handles all game logic
        private RequestReply Play()
        {
            RequestReply conv = null;

            if(Balloons.Where(x => !x.IsFilled).Count() < 5)
            {
                conv = CommSubsystem.ConversationFactory.CreateFromConversationType<BuyBalloonConversation>();
                conv.ToProcessId = BalloonStores[0].ProcessId;
            }
            else if(Balloons.Where(x => x.IsFilled).Count() < 5)
            {
                conv = CommSubsystem.ConversationFactory.CreateFromConversationType<FillBalloonConversation>();
                conv.ToProcessId = WaterSources[0].ProcessId;
            }
            else
            {
                conv = CommSubsystem.ConversationFactory.CreateFromConversationType<ThrowBalloonConversation>();
                conv.ToProcessId = OtherPlayers.Where(x => x.LifePoints > 0).First().ProcessId;
            }

            return conv;
        }

        // take care of all actions needed to finish a game
        private void EndGame()
        {
            Thread.Sleep(3000);
            CleanupSession();
        }

        public override void CleanupSession()
        {
            base.CleanupSession();
            PotentialGames = new List<GameInfo>();
            Pennies = new Stack<Penny>();
            Balloons = new List<Balloon>();
            Game = new GameInfo();
            GameData = new GameProcessData();
            PennyBankPublicKey = new PublicKey();
            WaterSources = new List<GameProcessData>();
            BalloonStores = new List<GameProcessData>();
            UmbrellaSuppliers = new List<GameProcessData>();
            OtherPlayers = new List<GameProcessData>();
            MyProcessInfo.Status = ProcessInfo.StatusCode.JoiningGame;
        }
    }
}
