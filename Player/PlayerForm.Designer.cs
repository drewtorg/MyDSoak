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
            this.processLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.endpointLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.processListView = new System.Windows.Forms.ListView();
            this.nameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.valueColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.gameIdLabel = new System.Windows.Forms.Label();
            this.gameStatusLabel = new System.Windows.Forms.Label();
            this.numPlayerLabel = new System.Windows.Forms.Label();
            this.numBSLabel = new System.Windows.Forms.Label();
            this.numWSLabel = new System.Windows.Forms.Label();
            this.numUSLabel = new System.Windows.Forms.Label();
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
            // processLabel
            // 
            this.processLabel.AutoSize = true;
            this.processLabel.Location = new System.Drawing.Point(116, 9);
            this.processLabel.Name = "processLabel";
            this.processLabel.Size = new System.Drawing.Size(35, 13);
            this.processLabel.TabIndex = 1;
            this.processLabel.Text = "label2";
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
            // endpointLabel
            // 
            this.endpointLabel.AutoSize = true;
            this.endpointLabel.Location = new System.Drawing.Point(116, 32);
            this.endpointLabel.Name = "endpointLabel";
            this.endpointLabel.Size = new System.Drawing.Size(35, 13);
            this.endpointLabel.TabIndex = 3;
            this.endpointLabel.Text = "label3";
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
            // processListView
            // 
            this.processListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.valueColumn});
            this.processListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.processListView.Location = new System.Drawing.Point(12, 75);
            this.processListView.Name = "processListView";
            this.processListView.Size = new System.Drawing.Size(220, 126);
            this.processListView.TabIndex = 6;
            this.processListView.UseCompatibleStateImageBehavior = false;
            this.processListView.View = System.Windows.Forms.View.Details;
            // 
            // nameColumn
            // 
            this.nameColumn.Text = "Property Name";
            this.nameColumn.Width = 90;
            // 
            // valueColumn
            // 
            this.valueColumn.Text = "Property Value";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(344, 18);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(61, 13);
            this.statusLabel.TabIndex = 7;
            this.statusLabel.Text = "statusLabel";
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
            this.panel1.Controls.Add(this.numUSLabel);
            this.panel1.Controls.Add(this.numWSLabel);
            this.panel1.Controls.Add(this.numBSLabel);
            this.panel1.Controls.Add(this.numPlayerLabel);
            this.panel1.Controls.Add(this.gameStatusLabel);
            this.panel1.Controls.Add(this.gameIdLabel);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(238, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(167, 126);
            this.panel1.TabIndex = 9;
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
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Game Status:";
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
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Number of BS\'s:";
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
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(4, 104);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(84, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Number of US\'s:";
            // 
            // gameIdLabel
            // 
            this.gameIdLabel.AutoSize = true;
            this.gameIdLabel.Location = new System.Drawing.Point(110, 4);
            this.gameIdLabel.Name = "gameIdLabel";
            this.gameIdLabel.Size = new System.Drawing.Size(47, 13);
            this.gameIdLabel.TabIndex = 6;
            this.gameIdLabel.Text = "Game: 4";
            // 
            // gameStatusLabel
            // 
            this.gameStatusLabel.AutoSize = true;
            this.gameStatusLabel.Location = new System.Drawing.Point(110, 24);
            this.gameStatusLabel.Name = "gameStatusLabel";
            this.gameStatusLabel.Size = new System.Drawing.Size(50, 13);
            this.gameStatusLabel.TabIndex = 7;
            this.gameStatusLabel.Text = "Available";
            // 
            // numPlayerLabel
            // 
            this.numPlayerLabel.AutoSize = true;
            this.numPlayerLabel.Location = new System.Drawing.Point(110, 44);
            this.numPlayerLabel.Name = "numPlayerLabel";
            this.numPlayerLabel.Size = new System.Drawing.Size(13, 13);
            this.numPlayerLabel.TabIndex = 8;
            this.numPlayerLabel.Text = "0";
            // 
            // numBSLabel
            // 
            this.numBSLabel.AutoSize = true;
            this.numBSLabel.Location = new System.Drawing.Point(110, 64);
            this.numBSLabel.Name = "numBSLabel";
            this.numBSLabel.Size = new System.Drawing.Size(13, 13);
            this.numBSLabel.TabIndex = 9;
            this.numBSLabel.Text = "0";
            // 
            // numWSLabel
            // 
            this.numWSLabel.AutoSize = true;
            this.numWSLabel.Location = new System.Drawing.Point(110, 84);
            this.numWSLabel.Name = "numWSLabel";
            this.numWSLabel.Size = new System.Drawing.Size(13, 13);
            this.numWSLabel.TabIndex = 10;
            this.numWSLabel.Text = "0";
            // 
            // numUSLabel
            // 
            this.numUSLabel.AutoSize = true;
            this.numUSLabel.Location = new System.Drawing.Point(110, 104);
            this.numUSLabel.Name = "numUSLabel";
            this.numUSLabel.Size = new System.Drawing.Size(13, 13);
            this.numUSLabel.TabIndex = 11;
            this.numUSLabel.Text = "0";
            // 
            // PlayerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 219);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.processListView);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.endpointLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.processLabel);
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
        private System.Windows.Forms.Label processLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label endpointLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView processListView;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label numUSLabel;
        private System.Windows.Forms.Label numWSLabel;
        private System.Windows.Forms.Label numBSLabel;
        private System.Windows.Forms.Label numPlayerLabel;
        private System.Windows.Forms.Label gameStatusLabel;
        private System.Windows.Forms.Label gameIdLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}