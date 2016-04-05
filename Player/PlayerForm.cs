using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Player
{
    public partial class PlayerForm : Form
    {
        public Player Player { get; set; }
        public Timer timer;
        private bool started { get; set; }

        public PlayerForm()
        {
            InitializeComponent();

            timer = new Timer();

            timer.Interval = 200;
            timer.Tick += Timer_Tick;

            ProcessLabel.Text =  "";
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
            if (Player != null)
            {
                ProcessLabel.Text = Player.MyProcessInfo.LabelAndId;
                EndpointLabel.Text = Player.MyProcessInfo.EndPoint?.ToString();
                StatusLabel.Text = Player.StatusString;


                if (Player.Game != null)
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
                        "Life Points",
                        Player.GameData.LifePoints.ToString()
                    }));
            ProcessListView.Items.Add(new ListViewItem(new string[]
            {
                        "Pennies",
                        Player.Pennies.Count.ToString()

            }));
            ProcessListView.Items.Add(new ListViewItem(new string[]
            {
                        "Unfilled Balloons",
                        Player.Balloons.Where(x => !x.IsFilled).Count().ToString()
            }));
            ProcessListView.Items.Add(new ListViewItem(new string[]
            {
                        "Filled Balloons",
                        Player.Balloons.Where(x => x.IsFilled).Count().ToString()
            }));
            ProcessListView.Items.Add(new ListViewItem(new string[]
            {
                        "Umbrellas",
                        "0"
            }));
            ProcessListView.Items.Add(new ListViewItem(new string[]
            {
                        "Has Umbrella Raised",
                        "False"
            }));
        }

        private void UpdateGameInfo()
        {
            GameIdLabel.Text = Player.Game.GameId.ToString();
            GameStatusLabel.Text = Player.Game.Status.ToString();
            NumWSLabel.Text = Player.WaterSources.Count.ToString();
            NumBSLabel.Text = Player.BalloonStores.Count.ToString();
            NumUSLabel.Text = Player.UmbrellaSuppliers.Count.ToString();
            NumPlayerLabel.Text = Player.OtherPlayers.Count.ToString();
        }

        private void PlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Player.Status == "Running")
            {
                timer.Stop();
                Player.Stop();
            }
        }

        private void StartStopButton_Click(object sender, EventArgs e)
        {
            if (!started)
            {
                if (Player != null)
                {
                    Player.Shutdown += Player_Shutdown;

                    Player.Start();
                    started = true;
                    StartButton.Enabled = false;
                    timer.Start();
                }
            }
        }

        private void Player_Shutdown(CommSub.StateChange changeInfo)
        {
            Close();
        }
    }
}
