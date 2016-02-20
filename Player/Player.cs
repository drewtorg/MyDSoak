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

namespace Player
{
    public class Player : CommProcess
    {
        public Player(PlayerOptions options)
        {
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

            CommSubsystem.Start();

            Conversation loginConv = CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(LoginConversation));
            loginConv.Start();
            while (loginConv.IsRunning) Thread.Sleep(0);

            Conversation gameListConv = CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(GameListConversation));
            gameListConv.Start();
            while (gameListConv.IsRunning) Thread.Sleep(0);

            Conversation joinGameConv = CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(JoinGameConversation));
            joinGameConv.Start();
            while (joinGameConv.IsRunning) Thread.Sleep(0);

            while(KeepGoing)
            {
                Thread.Sleep(0);
            }
        }
    }
}
