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

        public Label ProcessLabel { get { return processLabel; } set{ processLabel = value; } }
        public Label EndpointLabel { get { return endpointLabel; } set { endpointLabel = value; } }
        public Label StatusLabel { get { return statusLabel; } set { statusLabel = value; } }
        public Label GameIdLabel { get { return gameIdLabel; } set { gameIdLabel = value; } }
        public Label GameStatusLabel { get { return gameStatusLabel; } set { gameStatusLabel = value; } }
        public Label NumPlayerLabel { get { return numPlayerLabel; } set { numPlayerLabel = value; } }
        public Label NumBSLabel { get { return numBSLabel; } set { numBSLabel = value; } }
        public Label NumWSLabel { get { return numWSLabel; } set { numWSLabel = value; } }
        public Label NumUSLabel { get { return numUSLabel; } set { numUSLabel = value; } }
        public ListView ProcessListView { get { return processListView; } set { processListView = value; } }

        public PlayerForm(Player player)
        {
            InitializeComponent();

            Player = player;
            Player.Form = this;

            processLabel.Text =  "";
            endpointLabel.Text = "";
            statusLabel.Text = "Initializing";

            gameIdLabel.Text = "";
            gameStatusLabel.Text = "";
            numPlayerLabel.Text = "";
            numBSLabel.Text = "";
            numWSLabel.Text = "";
            numUSLabel.Text = "";

            player.Start();
        }

        private void PlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player.SendLogoutRequest();
        }
    }
}
