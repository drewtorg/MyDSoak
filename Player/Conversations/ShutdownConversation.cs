using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ReceiverConversations;

using Messages;
using Messages.RequestMessages;

namespace Player.Conversations
{
    public class ShutdownConversation : Receiver
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                return new[] { typeof(ShutdownRequest) };
            }
        }

        protected override void HandleRequest(Message request)
        {
            Process.BeginShutdown();
        }
    }
}
