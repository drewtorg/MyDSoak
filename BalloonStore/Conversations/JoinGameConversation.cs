using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommSub;
using CommSub.Conversations.InitiatorConversations;
using Messages.RequestMessages;
using Messages.ReplyMessages;
using log4net;
using SharedObjects;
using Messages;

namespace BalloonStore.Conversations
{
    public class JoinGameConversation : RequestReply
    {
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new Type[] { typeof(JoinGameReply) };
            }
        }

        protected override Message CreateRequest()
        {
            return new JoinGameRequest()
            {
                Process = Process.MyProcessInfo,
                GameId = ((BalloonStore)Process).Options.GameId
            };
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() &&
                ((BalloonStore)Process).Options.GameId > 0;
        }

        protected override bool IsProcessStateValid()
        {
            return base.IsProcessStateValid() &&
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.JoiningGame;
        }

        protected override void ProcessReply(Reply reply)
        {
            if (reply.Success)
            {
                JoinGameReply joinReply = reply as JoinGameReply;

                //((Player)Process).GameData = new GameProcessData()
                //{
                //    HasUmbrellaRaised = false,
                //    HitPoints = 0,
                //    LifePoints = joinReply.InitialLifePoints,
                //    Type = ProcessInfo.ProcessType.Player,
                //    ProcessId = joinReply.MsgId.Pid,
                //};
                ((BalloonStore)Process).Game = new GameInfo()
                {
                    GameId = ((BalloonStore)Process).Options.GameId,
                    GameManagerId = ((BalloonStore)Process).Options.GameManagerId,
                    Status = GameInfo.StatusCode.Available
                };
                Process.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
            }
        }
    }
}
