using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utils;

using SharedObjects;

namespace CommSub
{
    public abstract class CommProcess : BackgroundThread
    {
        public CommProcessState State { get; set; }
        public RuntimeOptions Options { get; set; }
        public CommSubsystem CommSubsystem { get; set; }

        protected override void Process(object state)
        {
            while (KeepGoing)
            {
                State.Do();
            }
        }

        public override void Start()
        {
            base.Start();

            CommSubsystem.Start();
        }

        public override void Stop()
        {
            CommSubsystem.Stop();

            base.Stop();

        }
    }
}
