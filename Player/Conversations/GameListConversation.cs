using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations.InitiatorConversations;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;
using log4net;
using Player.States;
using Messages;

namespace Player.Conversations
{
    public class GameListConversation : RequestReply//: InitiatorRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GameListConversation));

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
        //    Label = "GameListConversation";
        //    SendTo = PlayerState.RegistryEndPoint;
        //}

        //protected override Request CreateRequest()
        //{
        //    GameListRequest request = new GameListRequest()
        //    {
        //        StatusFilter = (int)GameInfo.StatusCode.Available
        //    };
        //    request.InitMessageAndConversationNumbers();
        //    return request;
        //}

        //protected override void ProcessFailure()
        //{
        //    Logger.Warn("GameListConversation Failed");
        //}

        //protected override bool ProcessReply(Envelope envelope)
        //{
        //    GameListReply reply = envelope.Message as GameListReply;


        //    PlayerState.Process.Status = ProcessInfo.StatusCode.JoiningGame;
        //    PlayerState.PotentialGames = reply.GameInfo.ToList();

        //    return PlayerState.PotentialGames.Count > 0;
        //}

        //protected override bool ValidateProcessState()
        //{
        //    return PlayerState is RegisteredPlayerState;
        //}
    }
}
