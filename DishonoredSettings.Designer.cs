namespace LiveSplit.Dishonored
{
    partial class DishonoredSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.gbAutoSplit = new System.Windows.Forms.GroupBox();
            this.tlpAutoSplit = new System.Windows.Forms.TableLayoutPanel();
            this.chkAutoStartEnd = new System.Windows.Forms.CheckBox();
            this.chkAutoSplitIntroEnd = new System.Windows.Forms.CheckBox();
            this.chkAutoSplitMissionEnd = new System.Windows.Forms.CheckBox();
            this.chkAutoSplitPrisonEscape = new System.Windows.Forms.CheckBox();
            this.chkAutoSplitWeepers = new System.Windows.Forms.CheckBox();
            this.chkAutoSplitOutsidersDream = new System.Windows.Forms.CheckBox();
            this.gbLoadRemoval = new System.Windows.Forms.GroupBox();
            this.tlpLoadRemoval = new System.Windows.Forms.TableLayoutPanel();
            this.chkDisplayWithoutLoads = new System.Windows.Forms.CheckBox();
            this.tlpMain.SuspendLayout();
            this.gbAutoSplit.SuspendLayout();
            this.tlpAutoSplit.SuspendLayout();
            this.gbLoadRemoval.SuspendLayout();
            this.tlpLoadRemoval.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.gbAutoSplit, 1, 0);
            this.tlpMain.Controls.Add(this.gbLoadRemoval, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 2;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpMain.Size = new System.Drawing.Size(476, 226);
            this.tlpMain.TabIndex = 0;
            // 
            // gbAutoSplit
            // 
            this.gbAutoSplit.Controls.Add(this.tlpAutoSplit);
            this.gbAutoSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAutoSplit.Location = new System.Drawing.Point(3, 3);
            this.gbAutoSplit.Name = "gbAutoSplit";
            this.gbAutoSplit.Size = new System.Drawing.Size(470, 163);
            this.gbAutoSplit.TabIndex = 5;
            this.gbAutoSplit.TabStop = false;
            this.gbAutoSplit.Text = "Auto-Split";
            // 
            // tlpAutoSplit
            // 
            this.tlpAutoSplit.ColumnCount = 1;
            this.tlpAutoSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAutoSplit.Controls.Add(this.chkAutoStartEnd, 0, 0);
            this.tlpAutoSplit.Controls.Add(this.chkAutoSplitIntroEnd, 0, 1);
            this.tlpAutoSplit.Controls.Add(this.chkAutoSplitMissionEnd, 0, 2);
            this.tlpAutoSplit.Controls.Add(this.chkAutoSplitPrisonEscape, 0, 3);
            this.tlpAutoSplit.Controls.Add(this.chkAutoSplitWeepers, 0, 5);
            this.tlpAutoSplit.Controls.Add(this.chkAutoSplitOutsidersDream, 0, 4);
            this.tlpAutoSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAutoSplit.Location = new System.Drawing.Point(3, 16);
            this.tlpAutoSplit.Name = "tlpAutoSplit";
            this.tlpAutoSplit.RowCount = 6;
            this.tlpAutoSplit.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAutoSplit.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAutoSplit.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAutoSplit.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAutoSplit.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAutoSplit.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAutoSplit.Size = new System.Drawing.Size(464, 144);
            this.tlpAutoSplit.TabIndex = 0;
            // 
            // chkAutoStartEnd
            // 
            this.chkAutoStartEnd.AutoSize = true;
            this.chkAutoStartEnd.Checked = true;
            this.chkAutoStartEnd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoStartEnd.Location = new System.Drawing.Point(3, 3);
            this.chkAutoStartEnd.Name = "chkAutoStartEnd";
            this.chkAutoStartEnd.Size = new System.Drawing.Size(117, 17);
            this.chkAutoStartEnd.TabIndex = 4;
            this.chkAutoStartEnd.Text = "Start / Reset / End";
            this.chkAutoStartEnd.UseVisualStyleBackColor = true;
            // 
            // chkAutoSplitIntroEnd
            // 
            this.chkAutoSplitIntroEnd.AutoSize = true;
            this.chkAutoSplitIntroEnd.Location = new System.Drawing.Point(3, 26);
            this.chkAutoSplitIntroEnd.Name = "chkAutoSplitIntroEnd";
            this.chkAutoSplitIntroEnd.Size = new System.Drawing.Size(69, 17);
            this.chkAutoSplitIntroEnd.TabIndex = 5;
            this.chkAutoSplitIntroEnd.Text = "Intro End";
            this.chkAutoSplitIntroEnd.UseVisualStyleBackColor = true;
            // 
            // chkAutoSplitMissionEnd
            // 
            this.chkAutoSplitMissionEnd.AutoSize = true;
            this.chkAutoSplitMissionEnd.Location = new System.Drawing.Point(3, 49);
            this.chkAutoSplitMissionEnd.Name = "chkAutoSplitMissionEnd";
            this.chkAutoSplitMissionEnd.Size = new System.Drawing.Size(83, 17);
            this.chkAutoSplitMissionEnd.TabIndex = 1;
            this.chkAutoSplitMissionEnd.Text = "Mission End";
            this.chkAutoSplitMissionEnd.UseVisualStyleBackColor = true;
            // 
            // chkAutoSplitPrisonEscape
            // 
            this.chkAutoSplitPrisonEscape.AutoSize = true;
            this.chkAutoSplitPrisonEscape.Location = new System.Drawing.Point(3, 72);
            this.chkAutoSplitPrisonEscape.Name = "chkAutoSplitPrisonEscape";
            this.chkAutoSplitPrisonEscape.Size = new System.Drawing.Size(179, 17);
            this.chkAutoSplitPrisonEscape.TabIndex = 2;
            this.chkAutoSplitPrisonEscape.Text = "Prison Escape (Sewer Entrance)";
            this.chkAutoSplitPrisonEscape.UseVisualStyleBackColor = true;
            // 
            // chkAutoSplitWeepers
            // 
            this.chkAutoSplitWeepers.AutoSize = true;
            this.chkAutoSplitWeepers.Location = new System.Drawing.Point(3, 118);
            this.chkAutoSplitWeepers.Name = "chkAutoSplitWeepers";
            this.chkAutoSplitWeepers.Size = new System.Drawing.Size(69, 17);
            this.chkAutoSplitWeepers.TabIndex = 3;
            this.chkAutoSplitWeepers.Text = "Weepers";
            this.chkAutoSplitWeepers.UseVisualStyleBackColor = true;
            // 
            // chkAutoSplitOutsidersDream
            // 
            this.chkAutoSplitOutsidersDream.AutoSize = true;
            this.chkAutoSplitOutsidersDream.Location = new System.Drawing.Point(3, 95);
            this.chkAutoSplitOutsidersDream.Name = "chkAutoSplitOutsidersDream";
            this.chkAutoSplitOutsidersDream.Size = new System.Drawing.Size(106, 17);
            this.chkAutoSplitOutsidersDream.TabIndex = 6;
            this.chkAutoSplitOutsidersDream.Text = "Outsider\'s Dream";
            this.chkAutoSplitOutsidersDream.UseVisualStyleBackColor = true;
            // 
            // gbLoadRemoval
            // 
            this.gbLoadRemoval.Controls.Add(this.tlpLoadRemoval);
            this.gbLoadRemoval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbLoadRemoval.Location = new System.Drawing.Point(3, 172);
            this.gbLoadRemoval.Name = "gbLoadRemoval";
            this.gbLoadRemoval.Size = new System.Drawing.Size(470, 51);
            this.gbLoadRemoval.TabIndex = 6;
            this.gbLoadRemoval.TabStop = false;
            this.gbLoadRemoval.Text = "Show Alternate Timing Time";
            // 
            // tlpLoadRemoval
            // 
            this.tlpLoadRemoval.ColumnCount = 1;
            this.tlpLoadRemoval.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLoadRemoval.Controls.Add(this.chkDisplayWithoutLoads, 0, 0);
            this.tlpLoadRemoval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpLoadRemoval.Location = new System.Drawing.Point(3, 16);
            this.tlpLoadRemoval.Name = "tlpLoadRemoval";
            this.tlpLoadRemoval.RowCount = 1;
            this.tlpLoadRemoval.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLoadRemoval.Size = new System.Drawing.Size(464, 32);
            this.tlpLoadRemoval.TabIndex = 0;
            // 
            // chkDisplayWithoutLoads
            // 
            this.chkDisplayWithoutLoads.AutoSize = true;
            this.chkDisplayWithoutLoads.Checked = true;
            this.chkDisplayWithoutLoads.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDisplayWithoutLoads.Location = new System.Drawing.Point(3, 3);
            this.chkDisplayWithoutLoads.Name = "chkDisplayWithoutLoads";
            this.chkDisplayWithoutLoads.Size = new System.Drawing.Size(59, 17);
            this.chkDisplayWithoutLoads.TabIndex = 0;
            this.chkDisplayWithoutLoads.Text = "Enable";
            this.chkDisplayWithoutLoads.UseVisualStyleBackColor = true;
            // 
            // DishonoredSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "DishonoredSettings";
            this.Size = new System.Drawing.Size(476, 226);
            this.tlpMain.ResumeLayout(false);
            this.gbAutoSplit.ResumeLayout(false);
            this.tlpAutoSplit.ResumeLayout(false);
            this.tlpAutoSplit.PerformLayout();
            this.gbLoadRemoval.ResumeLayout(false);
            this.tlpLoadRemoval.ResumeLayout(false);
            this.tlpLoadRemoval.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.CheckBox chkDisplayWithoutLoads;
        private System.Windows.Forms.CheckBox chkAutoSplitMissionEnd;
        private System.Windows.Forms.CheckBox chkAutoSplitPrisonEscape;
        private System.Windows.Forms.CheckBox chkAutoSplitWeepers;
        private System.Windows.Forms.CheckBox chkAutoStartEnd;
        private System.Windows.Forms.GroupBox gbAutoSplit;
        private System.Windows.Forms.TableLayoutPanel tlpAutoSplit;
        private System.Windows.Forms.GroupBox gbLoadRemoval;
        private System.Windows.Forms.TableLayoutPanel tlpLoadRemoval;
        private System.Windows.Forms.CheckBox chkAutoSplitIntroEnd;
        private System.Windows.Forms.CheckBox chkAutoSplitOutsidersDream;
    }
}
