using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ReceiverConversations;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace Player.Conversations
{
    public class GameStatusConversation : Receiver
    {
        protected override Type[] AllowedTypes
        {
            get
            {
                return new[] { typeof(GameStatusNotification) };
            }
        }

        protected override void HandleRequest(Request request)
        {
            GameStatusNotification status = request as GameStatusNotification;
            GameProcessData[] allProcesses = status.Game.CurrentProcesses;
            ((Player)Process).WaterSources = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.WaterServer).ToList();
            ((Player)Process).BalloonStores = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.BalloonStore).ToList();
            ((Player)Process).UmbrellaSuppliers = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.UmbrellaSupplier).ToList();
            ((Player)Process).OtherPlayers = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.Player && x.ProcessId != Process.MyProcessInfo.ProcessId).ToList();
        }
    }
}
