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
    public class JoiningGamePlayerState : PlayerState
    {
        public JoiningGamePlayerState(PlayerState other) : base(other)
        {
            Process.Status = ProcessInfo.StatusCode.JoiningGame;
        }

        public override void Do()
        {
            JoinGameConversation joinGameConv = Player.CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(JoinGameConversation)) as JoinGameConversation;
            joinGameConv.PlayerState = this;
            joinGameConv.Start();
            while (joinGameConv.Status == "Running") Thread.Sleep(0);

            if (joinGameConv.Successful)
                Player.State = new JoinedGamePlayerState(this);
        }
    }
}
