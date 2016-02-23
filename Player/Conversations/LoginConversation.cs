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
using Player.States;

namespace Player.Conversations
{
    public class LoginConversation : InitiatorRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginConversation));

        public PlayerState PlayerState { get; set; }

        protected override void Initialize()
        {
            Label = "LoginConversation";
            SendTo = PlayerState.RegistryEndPoint;
        }

        protected override Request CreateRequest()
        {
            return new LoginRequest()
            {
                ConvId = new MessageNumber() { Pid = 0, Seq = 1 },
                MsgId = new MessageNumber() { Pid = 0, Seq = 1 },
                Identity = PlayerState.Identity,
                ProcessLabel = PlayerState.Process.Label,
                ProcessType = PlayerState.Process.Type
            };
        }

        protected override void ProcessFailure()
        {
            Logger.Warn("Registry did not respond to Login");
        }

        protected override bool ProcessReply(Envelope envelope)
        {
            LoginReply reply = envelope.Message as LoginReply;

            if (reply.Success)
            {
                PlayerState.Process = reply.ProcessInfo;
                MessageNumber.LocalProcessId = reply.ProcessInfo.ProcessId;
            }
            return reply.Success;
        }

        protected override bool ValidateProcessState()
        {
            return PlayerState is InitializingPlayerState;
        }
    }
}
