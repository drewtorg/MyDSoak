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
        }

        public Envelope Receive(int timeout)
        {
            //return EnvelopeQueueDictionary.Instance.GetByConversationId(convId).Dequeue(timeout);
            client.Client.ReceiveTimeout = timeout;
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] bytes = client.Receive(ref ep);

            if (bytes != null)
            {
                return new Envelope()
                {
                    Message = Message.Decode(bytes),
                    Ep = new PublicEndPoint()
                    {
                        IPEndPoint = ep
                    }
                };
            }
            return null;
        }
    }
}
