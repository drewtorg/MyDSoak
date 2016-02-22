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
                ProcessLabel = "Drew Torgeson",
                ProcessType = ProcessInfo.ProcessType.Player
            };
        }

        protected override void ProcessFailure()
        {
            Logger.Debug("Registry did not respond to Login");
        }

        protected override bool ProcessReply(Envelope envelope)
        {
            LoginReply reply = envelope.Message as LoginReply;

            PlayerState.Process = reply.ProcessInfo;
            MessageNumber.LocalProcessId = reply.ProcessInfo.ProcessId;
            return true;
        }

        protected override bool ValidateProcessState()
        {
            return true;
        }
    }
}
