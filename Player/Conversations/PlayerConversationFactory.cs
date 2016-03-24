using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;

using Messages.RequestMessages;
using Messages.ReplyMessages;


namespace Player.Conversations
{
    public class PlayerConversationFactory : ConversationFactory
    {
        public override void Initialize()
        {
            Add(typeof(AliveRequest), typeof(AliveConversation));
            Add(typeof(JoinGameRequest), typeof(JoinGameConversation));
            Add(typeof(GameListRequest), typeof(GameListConversation));
            Add(typeof(LoginRequest), typeof(LoginConversation));
            Add(typeof(LogoutRequest), typeof(LogoutConversation));
        }

        //public override T CreateFromConversationType<T>(Envelope envelope = null)
        //{
        //    return base.CreateFromConversationType<T>(envelope);
        //}

        //public override Conversation CreateFromMessageType(Type messageType, Envelope envelope)
        //{
        //    return base.CreateFromMessageType(messageType, envelope);
        //}
    }
}
