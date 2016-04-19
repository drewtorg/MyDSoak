using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using CommSub;
using SharedObjects;
using BalloonStore.Conversations;
using CommSub.Conversations.InitiatorConversations;

namespace BalloonStore
{
    public class BalloonStore : CommProcess
    {
        public IdentityInfo Identity { get; set; }
        public ResourceSet<Balloon> Balloons { get; set; }
        public PublicKey PennyBankPublicKey { get; set; }
        new public BalloonStoreOptions Options { get; set; }
        public List<GameProcessData> WaterSources { get; set; }
        public List<GameProcessData> BalloonStores { get; set; }
        public List<GameProcessData> UmbrellaSuppliers { get; set; }
        public List<GameProcessData> Players { get; set; }
        public GameInfo Game { get; set; }
        public List<Penny> CachedPennies { get; set; }

        private RSACryptoServiceProvider _rsa;

        public BalloonStore(BalloonStoreOptions options)
        {
            Options = options;
            Initialize();
        }

        public void Initialize()
        {
            MyProcessInfo = new ProcessInfo()
            {
                Status = ProcessInfo.StatusCode.Initializing,
                Type = ProcessInfo.ProcessType.BalloonStore,
                Label = Options.Label
            };

            RegistryEndPoint = new PublicEndPoint(Options.Registry);

            Identity = new IdentityInfo()
            {
                Alias = Options.Alias,
                ANumber = Options.ANumber,
                FirstName = Options.FirstName,
                LastName = Options.LastName
            };

            SetupCommSubsystem(new BalloonStoreConversationFactory()
            {
                DefaultMaxRetries = Options.Retries,
                DefaultTimeout = Options.Timeout,
                Process = this
            }, minPort: Options.MinPort, maxPort: Options.MaxPort);

            Balloons = new ResourceSet<Balloon>();
            Game = new GameInfo();
            PennyBankPublicKey = new PublicKey();
            WaterSources = new List<GameProcessData>();
            BalloonStores = new List<GameProcessData>();
            UmbrellaSuppliers = new List<GameProcessData>();
            Players = new List<GameProcessData>();
            Balloons = new ResourceSet<Balloon>();

            _rsa = new RSACryptoServiceProvider();
        }

        public void CreateBalloons()
        {
            if(PennyBankPublicKey != null)
            {
                RSAPKCS1SignatureFormatter rsaSigner = new RSAPKCS1SignatureFormatter(_rsa);
                rsaSigner.SetHashAlgorithm("SHA1");

                SHA1Managed hasher = new SHA1Managed();
                for(int i = 0; i < Options.NumBalloons; i++)
                {
                    byte[] bytes = Encoding.Unicode.GetBytes(i.ToString());
                    byte[] hash = hasher.ComputeHash(bytes);

                    Balloon balloon = new Balloon()
                    {
                        Id = i,
                        IsFilled = false,
                        DigitalSignature = rsaSigner.CreateSignature(hash)
                    };

                    Balloons.AddOrUpdate(balloon);
                }
            }
        }

        protected override void Process(object state)
        {
            while (Status == "Running")
            {
                Conversation conv = GetConversation();

                if (conv != null)
                    conv.Execute();
            }
        }

        protected Conversation GetConversation()
        {
            RequestReply conv = null;

            switch (MyProcessInfo.Status)
            {
                case ProcessInfo.StatusCode.Initializing:
                    conv = CommSubsystem.ConversationFactory.CreateFromConversationType<LoginConversation>();
                    conv.TargetEndPoint = RegistryEndPoint;
                    break;
                case ProcessInfo.StatusCode.Registered:
                    MyProcessInfo.Status = ProcessInfo.StatusCode.JoiningGame;
                    break;
                case ProcessInfo.StatusCode.JoiningGame:
                    conv = CommSubsystem.ConversationFactory.CreateFromConversationType<JoinGameConversation>();
                    conv.ToProcessId = Options.GameManagerId;
                    break;
                case ProcessInfo.StatusCode.PlayingGame:
                    if(Balloons.AvailableCount == 0 && Balloons.UsedCount == Options.NumBalloons)
                        conv = CommSubsystem.ConversationFactory.CreateFromConversationType<LeaveGameConversation>();
                    break;
                case ProcessInfo.StatusCode.LeavingGame:
                    BeginShutdown();
                    break;
                case ProcessInfo.StatusCode.Terminating:
                    Stop();
                    break;
            }
            return conv;
        }
    }
}
