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
    public class InitializingPlayerState : PlayerState
    {
        //public override void Do()
        //{
        //    Process = new ProcessInfo()
        //    {
        //        Status = ProcessInfo.StatusCode.Initializing,
        //        Type = ProcessInfo.ProcessType.Player,
        //        Label = "Drew Torgeson",
        //    };

        //    LoginConversation loginConv = Player.CommSubsystem.ConversationFactory.CreateFromConversationType(typeof(LoginConversation)) as LoginConversation;
        //    loginConv.PlayerState = this;
        //    loginConv.Start();
        //    while (loginConv.Status == "Running") Thread.Sleep(0);

        //    if (loginConv.Successful)
        //        Player.State = new RegisteredPlayerState(this);
        //}
    }
}
