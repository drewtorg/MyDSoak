using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;
using SharedObjects;
using Utils;

namespace Player
{
    public class PlayerState : CommProcessState
    {
        public PublicEndPoint RegistryEndPoint { get; set; }
        public ProcessInfo Process { get; set; }
        public GameInfo Game { get; set; }
        public IdentityInfo Identity { get; set; }
        public List<GameInfo> PotentialGames { get; set; }


    }
}
