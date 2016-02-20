using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations;

using Messages.ReplyMessages;
using Messages.RequestMessages;

using log4net;

namespace Player.Conversations
{
    public class AliveConversation : ResponderRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AliveConversation));

        protected override void ProcessFailure()
        {
            Logger.Debug("Alive Reply failed");
        }

        protected override void ProcessRequest()
        {
            // AliveRequest doesn't require any processing
        }

        protected override void SendReply()
        {
            Envelope reply = new Envelope()
            {
                Message = new Reply()
                {
                    ConvId = ReceivedEnvelope.Message.ConvId,
                    //TODO: figure out how to make a real message number
                    MsgId = ReceivedEnvelope.Message.ConvId,
                    Success = true
                },
                Ep = ReceivedEnvelope.Ep
            };

            CommSubsystem.Communicator.Send(reply);
            Logger.Debug("Sent an Alive Reply to the registry");
        }

        protected override bool ValidateProcessState()
        {
            return true;
        }
    }
}
