using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Player;

using CommSub.Conversations.InitiatorConversations;

namespace PlayerTesting
{
    public class TestPlayer : Player.Player
    {
        new public RequestReply Play()
        {
            return base.Play();
        }

        new public RequestReply GetConversation()
        {
            return base.GetConversation();
        }
    }
}

