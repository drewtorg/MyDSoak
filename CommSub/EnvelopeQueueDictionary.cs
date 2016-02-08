using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharedObjects;

namespace CommSub
{
    public class EnvelopeQueueDictionary
    {
        private static object myLock = new object();
        private static EnvelopeQueueDictionary instance = null;
        private ConcurrentDictionary<MessageNumber, EnvelopeQueue> dictionary;

        private EnvelopeQueueDictionary()
        {
            dictionary = new ConcurrentDictionary<MessageNumber, EnvelopeQueue>();
        }

        public static EnvelopeQueueDictionary Instance
        {
            get
            {
                lock(myLock)
                {
                    if (instance == null)
                        instance = new EnvelopeQueueDictionary();
                    return instance;
                }
            }
        }

        public EnvelopeQueue GetByConversationId(MessageNumber label)
        {
            EnvelopeQueue queue = null;
            dictionary.TryGetValue(label, out queue);
            return queue;
        }

        public void CreateQueue()
        {

        }

        public void CloseQueue()
        {

        }
    }
}
