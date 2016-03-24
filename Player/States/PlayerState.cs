using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;
using SharedObjects;
using Utils;

namespace Player.States
{
    public abstract class PlayerState //: CommProcessState
    {
        public Player Player { get; set; }
        public PublicEndPoint RegistryEndPoint { get; set; }
        public GameInfo Game { get; set; }
        public IdentityInfo Identity { get; set; }
        public List<GameInfo> PotentialGames { get; set; }

        public PlayerState() { }

        public PlayerState(PlayerState other)
        {
            Identity = other.Identity;
            Game = other.Game;
            Player = other.Player;
            PotentialGames = other.PotentialGames;
            //Process = other.Process;
            RegistryEndPoint = other.RegistryEndPoint;

        }
    }
}
