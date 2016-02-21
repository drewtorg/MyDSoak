using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;
using log4net;

namespace Player.Conversations
{
    public class GameListConversation : InitiatorRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GameListConversation));

        public Player Player { get; set; }

        protected override Request CreateRequest()
        {
            int pid = Player.PlayerState.Process.ProcessId;
            int seq = Player.PlayerState.IDGen.GetNextIdNumber();
            return new GameListRequest()
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
                StatusFilter = (int)GameInfo.StatusCode.Available
            };
        }

        protected override void ProcessFailure()
        {
            Logger.Debug("GameListConversation Failed");
        }

        protected override void ProcessReply(Envelope envelope)
        {
            GameListReply reply = envelope.Message as GameListReply;


            Player.PlayerState.Process.Status = ProcessInfo.StatusCode.JoiningGame;
            Player.PlayerState.PotentialGames = reply.GameInfo.ToList();
        }

        protected override bool ValidateProcessState()
        {
            return true;
        }
    }
}
