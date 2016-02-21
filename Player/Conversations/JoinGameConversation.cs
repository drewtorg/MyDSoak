using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using log4net;
using SharedObjects;

namespace Player.Conversations
{
    public class JoinGameConversation : InitiatorRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JoinGameConversation));

        public Player Player { get; set; }

        protected override Request CreateRequest()
        {
            int pid = Player.PlayerState.Process.ProcessId;
            int seq = Player.PlayerState.IDGen.GetNextIdNumber();
            GameInfo game = Player.PlayerState.PotentialGames[0];

            return new JoinGameRequest()
            {
                ConvId = new MessageNumber()
                {
                    Pid = pid,
                    Seq = seq
                },
                MsgId = new MessageNumber()
                {
                    Pid = pid,
                    Seq = seq
                },
                GameId = game.GameId,
                Player = Player.PlayerState.Process
            };
        }

        protected override void ProcessFailure()
        {
            Logger.Debug("JoinGameConversation Failed");

            Player.PlayerState.PotentialGames.RemoveAt(0);
        }

        protected override void ProcessReply(Envelope envelope)
        {
            JoinGameReply reply = envelope.Message as JoinGameReply;

            if (reply.Success)
            {
                Player.PlayerState.Process.Status = ProcessInfo.StatusCode.JoinedGame;
                Player.PlayerState.Game = Player.PlayerState.PotentialGames[0];
                Player.PlayerState.Process.LifePoints = (short)reply.InitialLifePoints;
            }
            else
            {
                Player.PlayerState.PotentialGames.RemoveAt(0);
            }
        }

        protected override bool ValidateProcessState()
        {
            return true;
        }
    }
}
