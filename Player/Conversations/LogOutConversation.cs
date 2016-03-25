using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations.InitiatorConversations;
using Messages.RequestMessages;
using SharedObjects;
using log4net;
using Messages.ReplyMessages;
using Player.States;
using Messages;

namespace Player.Conversations
{
    public class LogoutConversation : RequestReply
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LogoutConversation));

        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new Type[] { typeof(Reply) };
            }
        }

        protected override Message CreateRequest()
        {
            return new LoginRequest()
            {
                ProcessLabel = Process.Label,
                ProcessType = ProcessInfo.ProcessType.Player,
                Identity = ((Player)Process).Identity
            };
        }

        protected override bool IsProcessStateValid()
        {
            return Process.MyProcessInfo.Status != ProcessInfo.StatusCode.Initializing &&
                   Process.MyProcessInfo.Status != ProcessInfo.StatusCode.NotInitialized;
        }

        protected override void ProcessReply(Reply reply)
        {
            Process.MyProcessInfo.Status = ProcessInfo.StatusCode.Terminating;
            Process.BeginShutdown();
        }
    }
}
