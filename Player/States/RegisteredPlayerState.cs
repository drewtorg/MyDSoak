using CommSub;
using Player.Conversations;
using SharedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Player.States
{
    public class RegisteredPlayerState : PlayerState
    {
        public RegisteredPlayerState(PlayerState other) : base(other)
        {
            Process.Status = ProcessInfo.StatusCode.Registered;
        }

        public override void Do()
        {
            GameListConversation gameListConv = Player.CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(GameListConversation)) as GameListConversation;
            gameListConv.PlayerState = this;
            gameListConv.Start();
            while (gameListConv.Status == "Running") Thread.Sleep(0);

            if (gameListConv.Successful)
                Player.State = new JoiningGamePlayerState(this);
        }
    }
}
