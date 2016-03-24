using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommSub;
using CommSub.Conversations;

using Utils;

using SharedObjects;

using Player.Conversations;
using log4net;
using Player.States;

namespace Player
{
    public class Player : CommProcess
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(Player));

        public Player()
        {
            Label = "Player";
            SetupCommSubsystem(new PlayerConversationFactory()
            {
                DefaultMaxRetries = Options.Retries,
                DefaultTimeout = Options.Timeout,
                Process = this
            });
        }

        protected override void Process(object state)
        {
            throw new NotImplementedException();
        }
    }
}
