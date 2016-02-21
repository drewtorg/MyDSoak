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
using Player.States;

namespace Player.Conversations
{
    public class LogoutConversation : InitiatorRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogoutConversation));

        public PlayerState PlayerState { get; set; }

        protected override void Initialize()
        {
            Label = "Logout Conversation";
            SendTo = PlayerState.RegistryEndPoint; 
        }

        protected override Request CreateRequest()
        {
            int pid = PlayerState.Process.ProcessId;
            int seq = PlayerState.IDGen.GetNextIdNumber();

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

        protected override bool ProcessReply(Envelope envelope)
        {
            Reply reply = envelope.Message as Reply;

            Logger.Debug("Successful Logout");
            return reply.Success;
        }

        protected override bool ValidateProcessState()
        {
            return true;
        }
    }
}
