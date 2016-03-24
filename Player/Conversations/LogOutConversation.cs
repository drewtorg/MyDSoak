using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations.InitiatorConversations;
using Messages.RequestMessages;
using SharedObjects;
using log4net;
using Messages.ReplyMessages;
using Player.States;
using Messages;

namespace Player.Conversations
{
    public class LogoutConversation : RequestReply//: InitiatorRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogoutConversation));

        protected override Type[] AllowedReplyTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override Message CreateRequest()
        {
            throw new NotImplementedException();
        }

        //public PlayerState PlayerState { get; set; }

        //protected override void Initialize()
        //{
        //    Label = "Logout Conversation";
        //    SendTo = PlayerState.RegistryEndPoint; 
        //}

        //protected override Request CreateRequest()
        //{
        //    LogoutRequest request = new LogoutRequest();
        //    request.InitMessageAndConversationNumbers();
        //    return request;
        //}

        //protected override void ProcessFailure()
        //{
        //    Logger.Warn("LogoutConversation Failed");
        //}

        //protected override bool ProcessReply(Envelope envelope)
        //{
        //    Reply reply = envelope.Message as Reply;
        //    PlayerState.Game = null;
        //    PlayerState.PotentialGames.Clear();

        //    Logger.Debug("Successful Logout");
        //    return reply.Success;
        //}

        //protected override bool ValidateProcessState()
        //{
        //    //a plyaer can log out in any state except the initializing state because it hasn't even logged in yet
        //    return !(PlayerState is InitializingPlayerState);
        //}
    }
}
