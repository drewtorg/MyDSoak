using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations;

using Messages.RequestMessages;
using Messages.ReplyMessages;


namespace Player.Conversations
{
    public class PlayerConversationFactory : ConversationFactory
    {
        protected override void InitTypeMappings()
        {
            AddTypeMapping(typeof(AliveRequest), typeof(AliveConversation));
            AddTypeMapping(typeof(JoinGameRequest), typeof(JoinGameConversation));
            AddTypeMapping(typeof(GameListRequest), typeof(GameListConversation));
            AddTypeMapping(typeof(LoginRequest), typeof(LoginConversation));
            AddTypeMapping(typeof(LogoutRequest), typeof(LogoutConversation));
        }
    }
}
