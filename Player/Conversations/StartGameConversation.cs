using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ReceiverConversations;

using Messages.ReplyMessages;
using SharedObjects;
using Messages.RequestMessages;

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

        protected override void HandleRequest(Request request)
        {
            Process.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
        }
    }
}
