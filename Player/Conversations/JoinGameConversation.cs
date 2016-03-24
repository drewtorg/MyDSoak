using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations.InitiatorConversations;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using log4net;
using SharedObjects;
using Player.States;
using Messages;

namespace Player.Conversations
{
    public class JoinGameConversation : RequestReply//: InitiatorRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JoinGameConversation));

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
        //    Label = "JoinGameConversation";
        //    SendTo = PlayerState.PotentialGames[0].GameManager.EndPoint;
        //}

        //protected override Request CreateRequest()
        //{
        //    GameInfo game = PlayerState.PotentialGames[0];

        //    JoinGameRequest request = new JoinGameRequest()
        //    {
        //        GameId = game.GameId,
        //        Player = PlayerState.Process
        //    };
        //    request.InitMessageAndConversationNumbers();
        //    return request;
        //}

        //protected override void ProcessFailure()
        //{
        //    Logger.Warn("JoinGameConversation Failed");

        //    PlayerState.PotentialGames.RemoveAt(0);
        //}

        //protected override bool ProcessReply(Envelope envelope)
        //{
        //    JoinGameReply reply = envelope.Message as JoinGameReply;

        //    if (reply.Success)
        //    {
        //        PlayerState.Process.Status = ProcessInfo.StatusCode.JoinedGame;
        //        PlayerState.Game = PlayerState.PotentialGames[0];
        //        PlayerState.Process.LifePoints = (short)reply.InitialLifePoints;
        //    }
        //    else
        //    {
        //        PlayerState.PotentialGames.RemoveAt(0);
        //    }

        //    return reply.Success;
        //}

        //protected override bool ValidateProcessState()
        //{
        //    return PlayerState is JoiningGamePlayerState;
        //}
    }
}
