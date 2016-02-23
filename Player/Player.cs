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

        public PlayerState PlayerState { get { return State as PlayerState; } }

        public Player(PlayerOptions options)
        {
            Label = "Player";
            Options = options;
            CommSubsystem = new CommSubsystem(new PlayerConversationFactory() { DefaultMaxRetries = 3, DefaultTimeOut = 3000 });
            CommSubsystem.Initialize();

            State = new InitializingPlayerState()
            {
                Identity = new IdentityInfo()
                {
                    Alias = options.Alias,
                    ANumber = options.ANumber,
                    FirstName = options.FirstName,
                    LastName = options.LastName
                },
                RegistryEndPoint = new PublicEndPoint(options.EndPoint),
                Player = this
            };
        }
    }
}
