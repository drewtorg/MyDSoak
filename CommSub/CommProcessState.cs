using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CommSub
{ 
    public class CommProcessState
    {
        public ObjectIdGenerator IDGen { get; }

        public CommProcessState()
        {
            IDGen = ObjectIdGenerator.Instance;
        }
    }
}
