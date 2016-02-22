﻿using SharedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CommSub
{ 
    public abstract class CommProcessState
    {
        public ProcessInfo Process { get; set; }

        public abstract void Do();
    }
}
