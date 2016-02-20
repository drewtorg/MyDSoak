using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations;
using log4net;

namespace CommSub
{
    public class CommSubsystem
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(CommSubsystem));

        public Communicator Communicator { get; set; }
        public EnvelopeQueueDictionary EnvelopeQueueDictionary { get; set; }
        public Dispatcher Dispatcher { get; set; }
        public ProcessAddressBook ProcessAddressBook { get; set; }
        public ConversationFactory ConversationFactory { get; set; }

        public void Initialize()
        {
            Logger.Debug("Initializing Commsubsystem");

            Communicator = new Communicator();
            EnvelopeQueueDictionary = new EnvelopeQueueDictionary();
            Dispatcher = new Dispatcher() { CommSubsystem = this, Label = "Dispatcher" };
            ProcessAddressBook = new ProcessAddressBook();
        }

        public void Start()
        {
            Logger.Debug("Starting Commsubsystem");
            Dispatcher.Start();
        }

        public void Stop()
        {
            Logger.Debug("Stopping CommSubsystem");
            Dispatcher.Stop();
        }
    }
}
