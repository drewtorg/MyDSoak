using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ResponderConversations;

using Messages;
using Messages.ReplyMessages;
using Messages.RequestMessages;
using SharedObjects;

namespace Player.Conversations
{
    public class ReadyToStartConversation : RequestReply
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                return new[] { typeof(ReadyToStart) };
            }
        }

        protected override Message CreateReply()
        {
            Process.MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
            return new Reply()
            {
                Note = "Let's do this!",
                Success = true
            };
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() && 
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.JoinedGame;
        }
    }
}
