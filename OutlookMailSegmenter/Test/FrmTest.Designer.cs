
namespace TMS.Apps.OutlookMailSegmenter.Test
{
    partial class FrmTest
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
            this.cbDataSource = new System.Windows.Forms.ComboBox();
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbFrom = new System.Windows.Forms.TextBox();
            this.tbTo = new System.Windows.Forms.TextBox();
            this.btnFetch = new System.Windows.Forms.Button();
            this.SFD = new System.Windows.Forms.SaveFileDialog();
            this.lbProgress = new System.Windows.Forms.Label();
            this.lbConnect = new System.Windows.Forms.Label();
            this.chbProcessAllReplaes = new System.Windows.Forms.CheckBox();
            this.chbProcessSignitures = new System.Windows.Forms.CheckBox();
            this.chbProcessHeaders = new System.Windows.Forms.CheckBox();
            this.chbProcessInParallel = new System.Windows.Forms.CheckBox();
            this.chbCheckForIdenticalChunks = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbDataSource
            // 
            this.cbDataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataSource.FormattingEnabled = true;
            this.cbDataSource.Location = new System.Drawing.Point(123, 72);
            this.cbDataSource.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbDataSource.Name = "cbDataSource";
            this.cbDataSource.Size = new System.Drawing.Size(212, 23);
            this.cbDataSource.TabIndex = 1;
            this.cbDataSource.SelectedIndexChanged += new System.EventHandler(this.cbDataSource_SelectedIndexChanged);
            // 
            // tvFolders
            // 
            this.tvFolders.Location = new System.Drawing.Point(13, 114);
            this.tvFolders.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tvFolders.Name = "tvFolders";
            this.tvFolders.Size = new System.Drawing.Size(321, 507);
            this.tvFolders.TabIndex = 2;
            this.tvFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFolders_AfterSelect);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(13, 23);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(104, 27);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect...";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 80);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Email store:";
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Location = new System.Drawing.Point(359, 120);
            this.lbCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(73, 15);
            this.lbCount.TabIndex = 5;
            this.lbCount.Text = "Email count:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(359, 181);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "from:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(547, 181);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "to:";
            // 
            // tbFrom
            // 
            this.tbFrom.Location = new System.Drawing.Point(403, 177);
            this.tbFrom.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbFrom.Name = "tbFrom";
            this.tbFrom.Size = new System.Drawing.Size(116, 23);
            this.tbFrom.TabIndex = 8;
            this.tbFrom.Text = "1";
            // 
            // tbTo
            // 
            this.tbTo.Location = new System.Drawing.Point(576, 177);
            this.tbTo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbTo.Name = "tbTo";
            this.tbTo.Size = new System.Drawing.Size(116, 23);
            this.tbTo.TabIndex = 8;
            this.tbTo.Text = "10";
            // 
            // btnFetch
            // 
            this.btnFetch.Location = new System.Drawing.Point(359, 259);
            this.btnFetch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(88, 27);
            this.btnFetch.TabIndex = 9;
            this.btnFetch.Text = "Fetch..";
            this.btnFetch.UseVisualStyleBackColor = true;
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
            // 
            // lbProgress
            // 
            this.lbProgress.AutoSize = true;
            this.lbProgress.Location = new System.Drawing.Point(530, 265);
            this.lbProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbProgress.Name = "lbProgress";
            this.lbProgress.Size = new System.Drawing.Size(38, 15);
            this.lbProgress.TabIndex = 11;
            this.lbProgress.Text = "status";
            // 
            // lbConnect
            // 
            this.lbConnect.AutoSize = true;
            this.lbConnect.Location = new System.Drawing.Point(168, 23);
            this.lbConnect.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbConnect.Name = "lbConnect";
            this.lbConnect.Size = new System.Drawing.Size(0, 15);
            this.lbConnect.TabIndex = 12;
            // 
            // chbProcessAllReplaes
            // 
            this.chbProcessAllReplaes.AutoSize = true;
            this.chbProcessAllReplaes.Checked = true;
            this.chbProcessAllReplaes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbProcessAllReplaes.Location = new System.Drawing.Point(360, 310);
            this.chbProcessAllReplaes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chbProcessAllReplaes.Name = "chbProcessAllReplaes";
            this.chbProcessAllReplaes.Size = new System.Drawing.Size(126, 19);
            this.chbProcessAllReplaes.TabIndex = 13;
            this.chbProcessAllReplaes.Text = "Process All Replaes";
            this.chbProcessAllReplaes.UseVisualStyleBackColor = true;
            this.chbProcessAllReplaes.CheckedChanged += new System.EventHandler(this.chbProcessAllReplaes_CheckedChanged);
            // 
            // chbProcessSignitures
            // 
            this.chbProcessSignitures.AutoSize = true;
            this.chbProcessSignitures.Checked = true;
            this.chbProcessSignitures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbProcessSignitures.Location = new System.Drawing.Point(360, 337);
            this.chbProcessSignitures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chbProcessSignitures.Name = "chbProcessSignitures";
            this.chbProcessSignitures.Size = new System.Drawing.Size(121, 19);
            this.chbProcessSignitures.TabIndex = 14;
            this.chbProcessSignitures.Text = "Process Signitures";
            this.chbProcessSignitures.UseVisualStyleBackColor = true;
            this.chbProcessSignitures.CheckedChanged += new System.EventHandler(this.chbProcessSignitures_CheckedChanged);
            // 
            // chbProcessHeaders
            // 
            this.chbProcessHeaders.AutoSize = true;
            this.chbProcessHeaders.Checked = true;
            this.chbProcessHeaders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbProcessHeaders.Location = new System.Drawing.Point(360, 363);
            this.chbProcessHeaders.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chbProcessHeaders.Name = "chbProcessHeaders";
            this.chbProcessHeaders.Size = new System.Drawing.Size(112, 19);
            this.chbProcessHeaders.TabIndex = 15;
            this.chbProcessHeaders.Text = "Process Headers";
            this.chbProcessHeaders.UseVisualStyleBackColor = true;
            this.chbProcessHeaders.CheckedChanged += new System.EventHandler(this.chbProcessHeaders_CheckedChanged);
            // 
            // chbProcessInParallel
            // 
            this.chbProcessInParallel.AutoSize = true;
            this.chbProcessInParallel.Checked = true;
            this.chbProcessInParallel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbProcessInParallel.Location = new System.Drawing.Point(572, 363);
            this.chbProcessInParallel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chbProcessInParallel.Name = "chbProcessInParallel";
            this.chbProcessInParallel.Size = new System.Drawing.Size(120, 19);
            this.chbProcessInParallel.TabIndex = 16;
            this.chbProcessInParallel.Text = "Process in Parallel";
            this.chbProcessInParallel.UseVisualStyleBackColor = true;
            this.chbProcessInParallel.CheckedChanged += new System.EventHandler(this.chbProcessInParallel_CheckedChanged);
            // 
            // chbCheckForIdenticalChunks
            // 
            this.chbCheckForIdenticalChunks.AutoSize = true;
            this.chbCheckForIdenticalChunks.Checked = true;
            this.chbCheckForIdenticalChunks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbCheckForIdenticalChunks.Location = new System.Drawing.Point(572, 310);
            this.chbCheckForIdenticalChunks.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chbCheckForIdenticalChunks.Name = "chbCheckForIdenticalChunks";
            this.chbCheckForIdenticalChunks.Size = new System.Drawing.Size(169, 19);
            this.chbCheckForIdenticalChunks.TabIndex = 17;
            this.chbCheckForIdenticalChunks.Text = "Check for Identical chunks.";
            this.chbCheckForIdenticalChunks.UseVisualStyleBackColor = true;
            this.chbCheckForIdenticalChunks.CheckedChanged += new System.EventHandler(this.chbCheckForIdenticalChunks_CheckedChanged_1);
            // 
            // FrmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 638);
            this.Controls.Add(this.chbCheckForIdenticalChunks);
            this.Controls.Add(this.chbProcessInParallel);
            this.Controls.Add(this.chbProcessHeaders);
            this.Controls.Add(this.chbProcessSignitures);
            this.Controls.Add(this.chbProcessAllReplaes);
            this.Controls.Add(this.lbConnect);
            this.Controls.Add(this.lbProgress);
            this.Controls.Add(this.btnFetch);
            this.Controls.Add(this.tbTo);
            this.Controls.Add(this.tbFrom);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.tvFolders);
            this.Controls.Add(this.cbDataSource);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FrmTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Segment Outlook Mail";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbDataSource;
        private System.Windows.Forms.TreeView tvFolders;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbFrom;
        private System.Windows.Forms.TextBox tbTo;
        private System.Windows.Forms.Button btnFetch;
        private System.Windows.Forms.SaveFileDialog SFD;
        private System.Windows.Forms.Label lbProgress;
        private System.Windows.Forms.Label lbConnect;
        private System.Windows.Forms.CheckBox chbProcessAllReplaes;
        private System.Windows.Forms.CheckBox chbProcessSignitures;
        private System.Windows.Forms.CheckBox chbProcessHeaders;
        private System.Windows.Forms.CheckBox chbProcessInParallel;
        private System.Windows.Forms.CheckBox chbCheckForIdenticalChunks;
    }
}