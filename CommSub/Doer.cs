using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utils;

namespace CommSub
{
    public class Doer : BackgroundThread
    {
        public CommSubsystem CommSubsystem { get; set; }

        protected override void Process(object state)
        {
            while(keepGoing)
            {
                // Do the things
            }
        }
    }
}
