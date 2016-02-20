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
    public class LoginConversation : InitiatorRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginConversation));

        public Player Player { get; set; }
        protected override Request CreateRequest()
        {
            return new LoginRequest()
            {
                ConvId = new MessageNumber() { Pid = 0, Seq = 1 },
                MsgId = new MessageNumber() { Pid = 0, Seq = 1 },
                Identity = Player.PlayerState.Identity,
                ProcessLabel = "Drew Torgeson",
                ProcessType = ProcessInfo.ProcessType.Player
            };
        }

        protected override void ProcessFailure()
        {
            Logger.Debug("Registry did not respond to Login");
        }

        protected override void ProcessReply(Envelope envelope)
        {
            LoginReply reply = envelope.Message as LoginReply;

            Player.PlayerState.Process = reply.ProcessInfo;
            Player.PlayerState.Process.Status = ProcessInfo.StatusCode.Registered;
        }

        protected override bool ValidateProcessState()
        {
            return true;
        }
    }
}
