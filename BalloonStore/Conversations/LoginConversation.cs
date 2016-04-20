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
using Messages;

namespace BalloonStore.Conversations
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
                Identity = ((BalloonStore)Process).Identity,
                ProcessLabel = Process.MyProcessInfo.Label,
                ProcessType = ProcessInfo.ProcessType.BalloonStore,
                PublicKey = ((BalloonStore)Process).PublicKey
            };
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() && Process.MyProcessInfo.Status == ProcessInfo.StatusCode.Initializing;
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() &&
                ((BalloonStore)Process).Identity != null;
        }

        protected override void ProcessReply(Reply reply)
        {
            LoginReply loginReply = reply as LoginReply;

            if (loginReply.Success)
            {
                Process.ProxyEndPoint = loginReply.ProxyEndPoint;
                ((BalloonStore)Process).PennyBankEndPoint = loginReply.PennyBankEndPoint;
                ((BalloonStore)Process).PennyBankPublicKey = loginReply.PennyBankPublicKey;
                Process.MyProcessInfo = loginReply.ProcessInfo;
                MessageNumber.LocalProcessId = loginReply.ProcessInfo.ProcessId;

                ((BalloonStore)Process).CreateBalloons();
            }
        }
    }
}
