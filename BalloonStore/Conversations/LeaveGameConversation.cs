using CommSub.Conversations.InitiatorConversations;
using Messages;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using System;
using SharedObjects;
using System.Linq;

namespace BalloonStore.Conversations
{
    public class LeaveGameConversation : RequestReply
    {
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new[] { typeof(Reply) };
            }
        }

        protected override Message CreateRequest()
        {
            Process.MyProcessInfo.Status = ProcessInfo.StatusCode.LeavingGame;
            ToProcessId = ((BalloonStore)Process).Game.GameManagerId;
            return new LeaveGameRequest();
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() &&
                (Process.MyProcessInfo.Status == ProcessInfo.StatusCode.JoinedGame ||
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame);
        }

        protected override void ProcessReply(Reply reply)
        {
            if (reply.Success)
            {
                Process.CleanupSession();
                Process.MyProcessInfo.Status = ProcessInfo.StatusCode.Terminating;
            }
        }
    }
}