using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations;
using Messages.RequestMessages;
using SharedObjects;
using log4net;
using Messages.ReplyMessages;

namespace Player.Conversations
{
    public class LogoutConversation : InitiatorRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogoutConversation));

        public Player Player { get; set; }

        protected override Request CreateRequest()
        {
            int pid = Player.PlayerState.Process.ProcessId;
            int seq = Player.PlayerState.IDGen.GetNextIdNumber();

            return new LogoutRequest()
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
                }
            };
        }

        protected override void ProcessFailure()
        {
            Logger.Debug("LogoutConversation Failed");
        }

        protected override void ProcessReply(Envelope envelope)
        {
            Reply reply = envelope.Message as Reply;

            if (reply.Success)
                Logger.Debug("Successful Logout");
        }

        protected override bool ValidateProcessState()
        {
            return true;
        }
    }
}
