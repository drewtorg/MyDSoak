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
            CommSubsystem = new CommSubsystem()
            {
                ConversationFactory = new PlayerConversationFactory() { DefaultMaxRetries = 3, DefaultTimeOut = 1000 } 
            };
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
            loginConv.CommSubsystem = CommSubsystem;
            loginConv.SendTo = PlayerState.RegistryEndPoint;
            loginConv.Start();
            while (loginConv.Status == "Running") Thread.Sleep(0);

            Console.WriteLine("Finished LoginConversation");

            //Conversation gameListConv = CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(GameListConversation));
            //gameListConv.Start();
            //while (gameListConv.IsRunning) Thread.Sleep(0);

            //Conversation joinGameConv = CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(JoinGameConversation));
            //joinGameConv.Start();
            //while (joinGameConv.IsRunning) Thread.Sleep(0);

            while (KeepGoing)
            {
                Thread.Sleep(0);
            }
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
