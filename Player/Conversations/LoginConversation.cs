using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations.InitiatorConversations;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using SharedObjects;
using log4net;
using Player.States;
using Messages;

namespace Player.Conversations
{
    public class LoginConversation : RequestReply//: InitiatorRRConversation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LoginConversation));

        //public PlayerState PlayerState { get; set; }
        
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new Type[] { typeof(LoginReply) };
            }
        }

        protected override Message CreateRequest()
        {
            TargetEndPoint = Process.RegistryEndPoint;
            return new LoginRequest()
            {
                Identity = ((Player)Process).Identity,
                ProcessLabel = Process.Label,
                ProcessType = ProcessInfo.ProcessType.Player
            };
        }

        protected override bool IsProcessStateValid()
        {
            return Process.MyProcessInfo.Status == ProcessInfo.StatusCode.Initializing;
        }

        protected override bool IsConversationStateValid()
        {
            return ((Player)Process).Identity != null;
        }

        protected override void ProcessReply(Reply reply)
        {
            LoginReply login = reply as LoginReply;

            if (login.Success)
            {
                Process.MyProcessInfo = login.ProcessInfo;
                MessageNumber.LocalProcessId = login.ProcessInfo.ProcessId;
            }
        }
    }
}
