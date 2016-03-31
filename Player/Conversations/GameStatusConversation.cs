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
            ((Player)Process).Game.Status = status.Game.Status;
            ((Player)Process).Game.Winners = status.Game.Winners;
            GameProcessData[] allProcesses = status.Game.CurrentProcesses;
            ((Player)Process).WaterSources = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.WaterServer).ToList();
            ((Player)Process).BalloonStores = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.BalloonStore).ToList();
            ((Player)Process).UmbrellaSuppliers = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.UmbrellaSupplier).ToList();
            ((Player)Process).OtherPlayers = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.Player && x.ProcessId != Process.MyProcessInfo.ProcessId).ToList();

            switch(status.Game.Status)
            {
                case GameInfo.StatusCode.Starting:
                    ((Player)Process).MyProcessInfo.Status = ProcessInfo.StatusCode.PlayingGame;
                    break;
                case GameInfo.StatusCode.Ending:
                    ((Player)Process).MyProcessInfo.Status = ProcessInfo.StatusCode.LeavingGame;
                    break;
                case GameInfo.StatusCode.Complete:
                    if (status.Game.Winners.Contains(Process.MyProcessInfo.ProcessId) && status.Game.Winners.Length > 1)
                        ((Player)Process).MyProcessInfo.Status = ProcessInfo.StatusCode.Tied;
                    else if (status.Game.Winners.Contains(Process.MyProcessInfo.ProcessId))
                        ((Player)Process).MyProcessInfo.Status = ProcessInfo.StatusCode.Won;
                    else
                        ((Player)Process).MyProcessInfo.Status = ProcessInfo.StatusCode.Lost;
                    break;
            }

        }
    }
}
