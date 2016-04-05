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

namespace Player.Conversations
{
    public class GameListConversation : RequestReply
    {
        protected override Type[] AllowedReplyTypes
        {
            get
            {
                return new Type[] { typeof(GameListReply) };
            }
        }

        protected override Message CreateRequest()
        {
            return new GameListRequest()
            {
                StatusFilter = (int)GameInfo.StatusCode.Available
            };
        }

        protected override bool IsProcessStateValid()
        {
            return Process.MyProcessInfo.Status == ProcessInfo.StatusCode.Registered;
        }

        protected override void ProcessReply(Reply reply)
        {
            if(reply.Success)
            {
                GameListReply listReply = reply as GameListReply;
                ((Player)Process).PotentialGames = listReply.GameInfo.ToList();
                Process.MyProcessInfo.Status = ProcessInfo.StatusCode.JoiningGame;
            }
        }
    }
}
