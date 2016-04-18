using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommSub.Conversations.ReceiverConversations;

using Messages;
using Messages.RequestMessages;
using SharedObjects;

namespace BalloonStore.Conversations
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

        protected override void HandleRequest(Message request)
        {
            GameStatusNotification status = request as GameStatusNotification;
            ((BalloonStore)Process).Game.Status = status.Game.Status;
            ((BalloonStore)Process).Game.Winners = status.Game.Winners;
            GameProcessData[] allProcesses = status.Game.CurrentProcesses;
            ((BalloonStore)Process).WaterSources = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.WaterServer).ToList();
            ((BalloonStore)Process).BalloonStores = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.BalloonStore).ToList();
            ((BalloonStore)Process).UmbrellaSuppliers = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.UmbrellaSupplier).ToList();
            ((BalloonStore)Process).Players = allProcesses.Where(x => x.Type == ProcessInfo.ProcessType.Player).ToList();

            switch (status.Game.Status)
            {
                case GameInfo.StatusCode.Ending:
                    ((BalloonStore)Process).MyProcessInfo.Status = ProcessInfo.StatusCode.LeavingGame;
                    break;
                case GameInfo.StatusCode.Complete:
                    ((BalloonStore)Process).MyProcessInfo.Status = ProcessInfo.StatusCode.LeavingGame;
                    break;
            }

        }

        protected override bool IsConversationStateValid()
        {
            return base.IsConversationStateValid() &&
                (Process.MyProcessInfo.Status == ProcessInfo.StatusCode.PlayingGame ||
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.JoinedGame ||
                Process.MyProcessInfo.Status == ProcessInfo.StatusCode.JoiningGame);
        }
    }
}
