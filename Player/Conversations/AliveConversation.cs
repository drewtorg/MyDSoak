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
using Messages;
using SharedObjects;

namespace Player.Conversations
{
    public class AliveConversation : ResponderRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AliveConversation));

        protected override void Initialize()
        {
            Label = "AliveConversation";
        }

        protected override void ProcessFailure()
        {
            Logger.Warn("Alive Reply failed");
        }

        protected override void ProcessRequest()
        {
            // AliveRequest doesn't require any processing
        }

        protected override Reply CreateReply()
        {
            return new Reply()
            {
                Success = true
            };
        }

        protected override bool ValidateProcessState()
        {
            // The player is always in a valid state to be alive
            return true;
        }
    }
}
