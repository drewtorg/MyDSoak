using log4net;
using Messages;
using SharedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommSub
{
    public class Communicator
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(Communicator));

        private UdpClient client;

        public Communicator()
        {
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 0);
            client = new UdpClient(localEp);
        }

        public void Send(Envelope envelope)
        {
            byte[] bytes = envelope.Message.Encode();
            client.Send(bytes, bytes.Length, envelope.Ep.IPEndPoint);

            Logger.Debug("Sending a Message: " + Encoding.ASCII.GetString(bytes));
        }

        public Envelope Receive(int timeout)
        {
            client.Client.ReceiveTimeout = timeout;
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = null;

            try {
                bytes = client.Receive(ref ep);
            }
            catch(Exception err)
            {
                Logger.Debug("There was no Message");
            }

            Envelope envelope = null;

            if (bytes != null)
            {
                Logger.Debug("Got a Message: " + Encoding.ASCII.GetString(bytes));

                envelope = new Envelope()
                {
                    Message = Message.Decode(bytes),
                    Ep = new PublicEndPoint()
                    {
                        IPEndPoint = ep
                    }
                };
            }
            return envelope;
        }
    }
}
