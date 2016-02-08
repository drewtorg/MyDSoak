using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Messages;
using SharedObjects;

namespace CommSub
{
    public class Envelope
    {
        public PublicEndPoint Ep { get; set; }
        public Message Message { get; set; }
    }
}
