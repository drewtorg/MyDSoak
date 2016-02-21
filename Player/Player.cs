using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommSub;
using CommSub.Conversations;

using Utils;

using SharedObjects;

using Player.Conversations;
using log4net;

namespace Player
{
    public class Player : CommProcess
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(Player));

        public PlayerState PlayerState { get { return State as PlayerState; } }

        public Player(PlayerOptions options)
        {
            Label = "Player";
            Options = options;
            CommSubsystem = new CommSubsystem(new PlayerConversationFactory() { DefaultMaxRetries = 3, DefaultTimeOut = 3000 });
            CommSubsystem.Initialize();

            State = new PlayerState()
            {
                Identity = new IdentityInfo()
                {
                    Alias = options.Alias,
                    ANumber = options.ANumber,
                    FirstName = options.FirstName,
                    LastName = options.LastName
                },
                RegistryEndPoint = new PublicEndPoint(options.EndPoint)
            };
        }

        protected override void Process(object state)
        {
            base.Process(state);

            Console.WriteLine("Starting LoginConversation");

            LoginConversation loginConv = CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(LoginConversation)) as LoginConversation;
            loginConv.Player = this;
            loginConv.Label = "LoginConversation";
            loginConv.SendTo = PlayerState.RegistryEndPoint;
            loginConv.Start();
            while (loginConv.Status == "Running") Thread.Sleep(0);

            Console.WriteLine("Finished LoginConversation");

            GameListConversation gameListConv = CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(GameListConversation)) as GameListConversation;
            gameListConv.Player = this;
            gameListConv.Label = "GameListConversation";
            gameListConv.SendTo = PlayerState.RegistryEndPoint;
            gameListConv.Start();
            while (gameListConv.Status == "Running") Thread.Sleep(0);

            JoinGameConversation joinGameConv = CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(JoinGameConversation)) as JoinGameConversation;
            joinGameConv.Player = this;
            joinGameConv.Label = "JoinGameConversation";
            joinGameConv.SendTo = PlayerState.PotentialGames[0].GameManager.EndPoint;
            joinGameConv.Start();
            while (joinGameConv.Status == "Running") Thread.Sleep(0);

            Thread.Sleep(5000);

            LogoutConversation logoutConv = CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(LogoutConversation)) as LogoutConversation;
            logoutConv.Player = this;
            logoutConv.Label = "LogoutConversation";
            logoutConv.SendTo = PlayerState.RegistryEndPoint;
            logoutConv.Start();
            while (logoutConv.Status == "Running") Thread.Sleep(0);

            Stop();
        }

        public override void Start()
        {
            base.Start();

            CommSubsystem.Start();
        }

        public override void Stop()
        {
            CommSubsystem.Stop();

            base.Stop();

        }
    }
}
