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
    public partial class PlayerForm : Form, IObserver
    {
        public Player Player { get; set; }
        private bool started { get; set; }

        public PlayerForm()
        {
            InitializeComponent();
            
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

        private void PlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Player != null)
                Player.SendLogoutRequest();
        }

        public void Update(ISubject subject)
        {
            if(subject is Player)
            {
                Player player = subject as Player;
                Player = player;

                UpdateDisplay();
            }
        }
        public void Remove(ISubject subject)
        {
            if(subject is Player && ((Player)subject).Equals(Player))
                Player = null;
        }

        private void UpdateDisplay()
        {
            if(Player != null)
            {
                ThreadHelper.SetText(this, ProcessLabel, Player.Process.Label);
                ThreadHelper.SetText(this, EndpointLabel, Player.Process.EndPoint.HostAndPort);
                ThreadHelper.SetText(this, StatusLabel, Player.Process.StatusString);

                if(Player.Game != null)
                {
                    ThreadHelper.SetText(this, GameIdLabel, Player.Game.GameId.ToString());
                    ThreadHelper.SetText(this, GameStatusLabel, Player.Game.Status.ToString());
                    ThreadHelper.ClearListView(this, ProcessListView);
                    ThreadHelper.AddListViewItem(this, ProcessListView, new ListViewItem(new string[]
                    {
                        "Life Points",
                        Player.Process.LifePoints.ToString()
                    }));
                }
            }

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
                }
            }
        }
    }
}
