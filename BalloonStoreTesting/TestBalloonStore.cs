using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BalloonStore;
using CommSub;
using CommSub.Conversations.InitiatorConversations;

namespace BalloonStoreTesting
{
    public class TestBalloonStore : BalloonStore.BalloonStore
    {
        public TestBalloonStore(BalloonStoreOptions options) : base(options) { }

        new public RequestReply GetConversation()
        {
            return base.GetConversation() as RequestReply;
        }
    }
}
