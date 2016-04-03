using System;
using CommSub;
using System.Threading;

namespace Player
{
    partial class PlayerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.ProcessLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.EndpointLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ProcessListView = new System.Windows.Forms.ListView();
            this.nameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.valueColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StatusLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.NumUSLabel = new System.Windows.Forms.Label();
            this.NumWSLabel = new System.Windows.Forms.Label();
            this.NumBSLabel = new System.Windows.Forms.Label();
            this.NumPlayerLabel = new System.Windows.Forms.Label();
            this.GameStatusLabel = new System.Windows.Forms.Label();
            this.GameIdLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Process Id / Label:";
            // 
            // ProcessLabel
            // 
            this.ProcessLabel.AutoSize = true;
            this.ProcessLabel.Location = new System.Drawing.Point(116, 9);
            this.ProcessLabel.Name = "ProcessLabel";
            this.ProcessLabel.Size = new System.Drawing.Size(35, 13);
            this.ProcessLabel.TabIndex = 1;
            this.ProcessLabel.Text = "label2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Public End Point:";
            // 
            // EndpointLabel
            // 
            this.EndpointLabel.AutoSize = true;
            this.EndpointLabel.Location = new System.Drawing.Point(116, 32);
            this.EndpointLabel.Name = "EndpointLabel";
            this.EndpointLabel.Size = new System.Drawing.Size(35, 13);
            this.EndpointLabel.TabIndex = 3;
            this.EndpointLabel.Text = "label3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Process State Info:";
            // 
            // ProcessListView
            // 
            this.ProcessListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.valueColumn});
            this.ProcessListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ProcessListView.Location = new System.Drawing.Point(12, 75);
            this.ProcessListView.Name = "ProcessListView";
            this.ProcessListView.Size = new System.Drawing.Size(220, 152);
            this.ProcessListView.TabIndex = 6;
            this.ProcessListView.UseCompatibleStateImageBehavior = false;
            this.ProcessListView.View = System.Windows.Forms.View.Details;
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Property Name";
            this.nameColumn.Width = 90;
            // 
            // valueColumn
            // 
            this.valueColumn.Text = "Property Value";
            this.valueColumn.Width = 87;
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(344, 18);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(61, 13);
            this.StatusLabel.TabIndex = 7;
            this.StatusLabel.Text = "statusLabel";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(235, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Game Info:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.NumUSLabel);
            this.panel1.Controls.Add(this.NumWSLabel);
            this.panel1.Controls.Add(this.NumBSLabel);
            this.panel1.Controls.Add(this.NumPlayerLabel);
            this.panel1.Controls.Add(this.GameStatusLabel);
            this.panel1.Controls.Add(this.GameIdLabel);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(238, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(214, 152);
            this.panel1.TabIndex = 9;
            // 
            // NumUSLabel
            // 
            this.NumUSLabel.AutoSize = true;
            this.NumUSLabel.Location = new System.Drawing.Point(110, 104);
            this.NumUSLabel.Name = "NumUSLabel";
            this.NumUSLabel.Size = new System.Drawing.Size(13, 13);
            this.NumUSLabel.TabIndex = 11;
            this.NumUSLabel.Text = "0";
            // 
            // NumWSLabel
            // 
            this.NumWSLabel.AutoSize = true;
            this.NumWSLabel.Location = new System.Drawing.Point(110, 84);
            this.NumWSLabel.Name = "NumWSLabel";
            this.NumWSLabel.Size = new System.Drawing.Size(13, 13);
            this.NumWSLabel.TabIndex = 10;
            this.NumWSLabel.Text = "0";
            // 
            // NumBSLabel
            // 
            this.NumBSLabel.AutoSize = true;
            this.NumBSLabel.Location = new System.Drawing.Point(110, 64);
            this.NumBSLabel.Name = "NumBSLabel";
            this.NumBSLabel.Size = new System.Drawing.Size(13, 13);
            this.NumBSLabel.TabIndex = 9;
            this.NumBSLabel.Text = "0";
            // 
            // NumPlayerLabel
            // 
            this.NumPlayerLabel.AutoSize = true;
            this.NumPlayerLabel.Location = new System.Drawing.Point(110, 44);
            this.NumPlayerLabel.Name = "NumPlayerLabel";
            this.NumPlayerLabel.Size = new System.Drawing.Size(13, 13);
            this.NumPlayerLabel.TabIndex = 8;
            this.NumPlayerLabel.Text = "0";
            // 
            // GameStatusLabel
            // 
            this.GameStatusLabel.AutoSize = true;
            this.GameStatusLabel.Location = new System.Drawing.Point(110, 24);
            this.GameStatusLabel.Name = "GameStatusLabel";
            this.GameStatusLabel.Size = new System.Drawing.Size(50, 13);
            this.GameStatusLabel.TabIndex = 7;
            this.GameStatusLabel.Text = "Available";
            // 
            // GameIdLabel
            // 
            this.GameIdLabel.AutoSize = true;
            this.GameIdLabel.Location = new System.Drawing.Point(110, 4);
            this.GameIdLabel.Name = "GameIdLabel";
            this.GameIdLabel.Size = new System.Drawing.Size(47, 13);
            this.GameIdLabel.TabIndex = 6;
            this.GameIdLabel.Text = "Game: 4";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 104);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(84, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Number of US\'s:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Number of WS\'s:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Number of BS\'s:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 44);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Number of Players:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Game Status:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Game Id:";
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(219, 9);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 10;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartStopButton_Click);
            // 
            // PlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 239);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.ProcessListView);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.EndpointLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ProcessLabel);
            this.Controls.Add(this.label1);
            this.Name = "PlayerForm";
            this.Text = "PlayerForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlayerForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Label ProcessLabel;
        public System.Windows.Forms.Label EndpointLabel;
        public System.Windows.Forms.ListView ProcessListView;
        public System.Windows.Forms.Label StatusLabel;
        public System.Windows.Forms.Label NumUSLabel;
        public System.Windows.Forms.Label NumWSLabel;
        public System.Windows.Forms.Label NumBSLabel;
        public System.Windows.Forms.Label NumPlayerLabel;
        public System.Windows.Forms.Label GameStatusLabel;
        public System.Windows.Forms.Label GameIdLabel;
        private System.Windows.Forms.Button StartButton;
    }
}