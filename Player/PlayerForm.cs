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

        public PlayerForm(Player player)
        {
            InitializeComponent();

            Player = player;
            Player.Form = this;

            ProcessLabel.Text =  "";
            EndpointLabel.Text = "";
            StatusLabel.Text = "Initializing";

            GameIdLabel.Text = "";
            GameStatusLabel.Text = "";
            NumPlayerLabel.Text = "";
            NumBSLabel.Text = "";
            NumWSLabel.Text = "";
            NumUSLabel.Text = "";

            player.Start();
        }

        private void PlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player.SendLogoutRequest();
        }

        public void Update(ISubject subject)
        {
            if(subject is Player)
            {

            }
        }

        public void Remove(ISubject subject)
        {
            if(subject is Player)
            {

            }
        }
    }
}
