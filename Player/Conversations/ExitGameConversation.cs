using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ResponderConversations;

using Messages.ReplyMessages;
using Messages.RequestMessages;

using log4net;
using Messages;
using SharedObjects;

namespace Player.Conversations
{
    public class ExitGameConversation : RequestReply
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                return new Type[]{ typeof(ExitGameRequest) };
            }
        }

        protected override Message CreateReply()
        {
            Process.MyProcessInfo.Status = ProcessInfo.StatusCode.LeavingGame;
            return new Reply()
            {
                Success = true,
                Note = "Okay..."
            };
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() && 
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame;
        }
    }
}
