using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;
using CommSub.Conversations;

using Utils;

using Player.Conversations;

namespace Player
{
    public class Player : CommProcess
    {
        public Player(RuntimeOptions options = null)
        {
            Options = options;
            CommSubsystem = new CommSubsystem()
            {
                ConversationFactory = new PlayerConversationFactory() { DefaultMaxRetries = 3, DefaultTimeOut = 1000 } 
            };

        }
    }
}
