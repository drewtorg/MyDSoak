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
using Player.States;
using Messages;

namespace Player.Conversations
{
    public class JoinGameConversation : RequestReply
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JoinGameConversation));

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
                Player = Process.MyProcessInfo,
                GameId = ((Player)Process).PotentialGames[0].GameId
            };
        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() &&
                ((Player)Process).PotentialGames.Count > 0;
        }

        protected override bool IsProcessStateValid()
        {
            return Process.MyProcessInfo.Status == ProcessInfo.StatusCode.JoiningGame;
        }

        protected override void ProcessReply(Reply reply)
        {
            if (reply.Success)
            {
                JoinGameReply joinReply = reply as JoinGameReply;

                ((Player)Process).GameData = new GameProcessData()
                {
                    HasUmbrellaRaised = false,
                    HitPoints = joinReply.InitialLifePoints,
                    LifePoints = joinReply.InitialLifePoints,
                    Type = ProcessInfo.ProcessType.Player,
                    ProcessId = joinReply.MsgId.Pid
                };
                Process.MyProcessInfo.Status = ProcessInfo.StatusCode.JoinedGame;
                ((Player)Process).PotentialGames.Clear();
            }
            else
            {
                ((Player)Process).PotentialGames.RemoveAt(0);
            }
        }
    }
}
