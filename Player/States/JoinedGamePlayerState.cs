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
    public class JoinedGamePlayerState : PlayerState
    {
        public JoinedGamePlayerState(PlayerState other) : base(other)
        {
            Process.Status = ProcessInfo.StatusCode.JoinedGame;
        }

        public override void Do()
        {
            Thread.Sleep(15000);

            Player.PlayerState.Process.Status = ProcessInfo.StatusCode.Terminating;

            LogoutConversation logoutConv = Player.CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(LogoutConversation)) as LogoutConversation;
            logoutConv.PlayerState = this;
            logoutConv.Start();
            while (logoutConv.Status == "Running") Thread.Sleep(0);

            if (logoutConv.Successful)
                Player.Stop();
        }
    }
}
