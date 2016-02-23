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
            if(Player != null)
            {
                ProcessLabel.Text = Player.PlayerState.Process.Label;
                EndpointLabel.Text = Player.PlayerState.Process.EndPoint.HostAndPort;
                StatusLabel.Text = Player.PlayerState.Process.StatusString;

                ProcessListView.Items.Clear();

                if (Player.PlayerState.Game != null)
                {
                    GameIdLabel.Text = Player.PlayerState.Game.GameId.ToString();
                    GameStatusLabel.Text = Player.PlayerState.Game.Status.ToString();
                    ProcessListView.Items.Add(new ListViewItem(new string[]
                    {
                        "Life Points",
                        Player.PlayerState.Process.LifePoints.ToString()
                    }));
                }
            }
        }

        private void PlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Stop();
            Player.Stop();

        }

        private void StartStopButton_Click(object sender, EventArgs e)
        {
            if (!started)
            {
                if (Player != null)
                {
                    Player.Start();
                    started = true;
                    StartButton.Enabled = false;
                    timer.Start();
                }
            }
        }
    }
}
