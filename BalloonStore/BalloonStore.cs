using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub;
using SharedObjects;
using BalloonStore.Conversations;

namespace BalloonStore
{
    public class BalloonStore : CommProcess
    {
        public ResourceSet<Balloon> Balloons { get; set; }
        public PublicKey PennyBankPublicKey { get; set; }
        new public BalloonStoreOptions Options { get; set; }

        public override void Start()
        {
            Initialize();
            base.Start();
        }

        public void Initialize()
        {
            MyProcessInfo.Status = ProcessInfo.StatusCode.Initializing;
            RegistryEndPoint = new PublicEndPoint(Options.RegistryEndPoint);
            SetupCommSubsystem(new BalloonStoreConversationFactory()
            {
                DefaultMaxRetries = Options.Retries,
                DefaultTimeout = Options.Timeout,
                Process = this
            }, minPort: Options.MinPort, maxPort: Options.MaxPort);
            CreateBalloons();
        }

        private void CreateBalloons()
        {

        }

        protected override void Process(object state)
        {
            while (Status == "Running")
            {
                Conversation conv = GetConversation();

                if (conv != null)
                    conv.Execute();
            }
        }

        protected Conversation GetConversation()
        {
            throw new NotImplementedException();
        }

        public override void CleanupSession()
        {

        }
    }
}
