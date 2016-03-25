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
        public PublicEndPoint PennyBankEndPoint { get; set; }
        public PublicKey PennyBankPublicKey { get; set; }
        public List<GameProcessData> WaterSources { get; set; }
        public List<GameProcessData> BalloonStores { get; set; }
        public List<GameProcessData> UmbrellaSuppliers { get; set; }
        public List<GameProcessData> OtherPlayers { get; set; }

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
            PotentialGames = new List<GameInfo>();
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
                        conv = Act();
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

        // This is the method that handles all game logic
        private RequestReply Act()
        {
            return null;
        }
    }
}
