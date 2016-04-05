using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ReceiverConversations;

using Messages.ReplyMessages;
using SharedObjects;
using Messages.RequestMessages;
using Messages;

namespace Player.Conversations
{
    public class StartGameConversation : Receiver
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                return new[] { typeof(StartGame) };
            }
        }

        protected override void HandleRequest(Message request)
        {
            StartGame start = request as StartGame;
            if (start.Success)
                Process.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            else
                Process.MyProcessInfo.Status = ProcessInfo.StatusCode.LeavingGame;
        }
    }
}
