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
        public List<GameProcessData> AlivePlayers { get { return OtherPlayers.Where(x => x.LifePoints > 0).ToList(); } }
        public Stack<Penny> Pennies { get; set; }
        public List<Balloon> Balloons { get; set; }

        public Player()
        {
            Label = "Drew's Player";
            MyProcessInfo = new ProcessInfo()
            {
                Status = ProcessInfo.StatusCode.NotInitialized,
                Label = Label,
                Type = ProcessInfo.ProcessType.Player
            };
            CleanupSession();
        }

        public override void Start()
        {
            Initialize();
            base.Start();
        }

        public void Initialize()
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
            }, minPort:Options.MinPort, maxPort:Options.MaxPort);
        }

        protected override void Process(object state)
        {
            while (Status == "Running")
            {
                Conversation conv = GetConversation();

                if (conv != null)
                {
                    conv.Launch();

                    while (!conv.Done)
                        Thread.Sleep(1000);
                }
            }
        }

        protected RequestReply GetConversation()
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
                    if (PotentialGames.Count > 0)
                    {
                        conv = CommSubsystem.ConversationFactory.CreateFromConversationType<JoinGameConversation>();
                        conv.ToProcessId = PotentialGames[0].GameManagerId;
                    }
                    else MyProcessInfo.Status = ProcessInfo.StatusCode.Registered; // maybe do this in the Conversation failure
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
            return conv;
        }

        // This is the method that handles all game logic
        protected RequestReply Play()
        {
            RequestReply conv = null;

            if (MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame)
            {
                if (Balloons.Count == 0 && BalloonStores.Count > 0)
                {
                    conv = CommSubsystem.ConversationFactory.CreateFromConversationType<BuyBalloonConversation>();
                    conv.ToProcessId = BalloonStores.First().ProcessId;
                }
                else if (Balloons.Where(x => !x.IsFilled).Count() > 0 && WaterSources.Count > 0)
                {
                    conv = CommSubsystem.ConversationFactory.CreateFromConversationType<FillBalloonConversation>();
                    conv.ToProcessId = WaterSources.First().ProcessId;
                }
                else if (AlivePlayers.Count > 0)
                {
                    conv = CommSubsystem.ConversationFactory.CreateFromConversationType<ThrowBalloonConversation>();
                    conv.ToProcessId = Game.GameManagerId;
                }
            }

            return conv;
        }

        // take care of all actions needed to finish a game
        protected void EndGame()
        {
            Thread.Sleep(3000);
            CleanupSession();
            MyProcessInfo.Status = ProcessInfo.StatusCode.Registered;
        }

        public override void CleanupSession()
        {
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
        }
    }
}
