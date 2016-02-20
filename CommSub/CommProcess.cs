using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utils;

namespace CommSub
{
    public class CommProcess : BackgroundThread
    {
        public CommProcessState State { get; set; }
        public RuntimeOptions Options { get; set; }
        public CommSubsystem CommSubsystem { get; set; }

        protected override void Process(object state)
        {
            
        }
    }
}
