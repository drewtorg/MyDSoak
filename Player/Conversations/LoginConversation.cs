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

namespace Player.Conversations
{
    public class LoginConversation : InitiatorRRConversation
    {
        public Player Player { get; set; }
        protected override Request CreateRequest()
        {
            return new LoginRequest()
            {
                ConvId = new MessageNumber() { Pid = 0, Seq = 1 },
                MsgId = new MessageNumber() { Pid = 0, Seq = 1 },
                Identity = ((PlayerState)Player.State).Identity,
                ProcessLabel = "Drew Torgeson",
                ProcessType = ProcessInfo.ProcessType.Player
            };
        }

        protected override void ProcessFailure()
        {
            Console.WriteLine("Something went wrong");
        }

        protected override void ProcessReply(Envelope envelope)
        {
            LoginReply reply = envelope.Message as LoginReply;

            ((PlayerState)Player.State).Process = reply.ProcessInfo;
            ((PlayerState)Player.State).Process.Status = ProcessInfo.StatusCode.Registered;
        }
    }
}
