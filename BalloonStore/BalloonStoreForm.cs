using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BalloonStore
{
    public partial class BalloonStoreForm : Form
    {
        public BalloonStore BalloonStore{ get; set; }
        public Timer timer;

        public BalloonStoreForm(BalloonStore store)
        {
            InitializeComponent();

            BalloonStore = store;

            BalloonStore.Shutdown += BalloonStore_Shutdown;

            timer = new Timer();

            timer.Interval = 200;
            timer.Tick += Timer_Tick;

            ProcessLabel.Text = "";
            EndpointLabel.Text = "";
            StatusLabel.Text = "Initializing";

            GameIdLabel.Text = "";
            GameStatusLabel.Text = "";
            NumPlayerLabel.Text = "";
            NumBSLabel.Text = "";
            NumWSLabel.Text = "";
            NumUSLabel.Text = "";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (BalloonStore != null)
            {
                ProcessLabel.Text = BalloonStore.MyProcessInfo.LabelAndId;
                EndpointLabel.Text = BalloonStore.MyProcessInfo.EndPoint?.ToString();
                StatusLabel.Text = BalloonStore.StatusString;


                if (BalloonStore.Game != null)
                {
                    UpdateProcessInfo();
                    UpdateGameInfo();
                }
            }
        }

        private void UpdateProcessInfo()
        {
            ProcessListView.Items.Clear();

            ProcessListView.Items.Add(new ListViewItem(new string[]
                    {
                        "Unfilled Balloons",
                        BalloonStore.Balloons.AvailableCount.ToString()
                    }));
        }

        private void UpdateGameInfo()
        {
            GameIdLabel.Text = BalloonStore.Game.GameId.ToString();
            GameStatusLabel.Text = BalloonStore.Game.Status.ToString();
            NumWSLabel.Text = BalloonStore.WaterSources.Count.ToString();
            NumBSLabel.Text = BalloonStore.BalloonStores.Count.ToString();
            NumUSLabel.Text = BalloonStore.UmbrellaSuppliers.Count.ToString();
            NumPlayerLabel.Text = BalloonStore.Players.Count.ToString();
        }

        private void BalloonStore_Shutdown(CommSub.StateChange changeInfo)
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(delegate { Close(); }));
            else
                Close();
        }

        private void BalloonStoreForm_Shown(object sender, EventArgs e)
        {
            timer.Start();
        }
    }
}
