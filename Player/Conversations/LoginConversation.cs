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
    public class LoginConversation : RequestReply
    {
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new Type[] { typeof(LoginReply) };
            }
        }

        protected override Message CreateRequest()
        {
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
            return base.IsConversationStateValid() &&
                ((Player)Process).Identity != null;
        }

        protected override void ProcessReply(Reply reply)
        {
            LoginReply loginReply = reply as LoginReply;

            if (loginReply.Success)
            {
                Process.ProxyEndPoint = loginReply.ProxyEndPoint;
                ((Player)Process).PennyBankEndPoint = loginReply.PennyBankEndPoint;
                ((Player)Process).PennyBankPublicKey = loginReply.PennyBankPublicKey;
                Process.MyProcessInfo = loginReply.ProcessInfo;
                MessageNumber.LocalProcessId = loginReply.ProcessInfo.ProcessId;
            }
        }
    }
}
