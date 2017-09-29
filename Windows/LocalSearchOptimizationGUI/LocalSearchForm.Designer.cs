namespace LocalSearchOptimizationGUI
{
    partial class LocalSearchForm
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
            this.menuBar = new System.Windows.Forms.MenuStrip();
            this.solverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localDescentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulatedAnnealingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.algorithmStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.costValueStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuBar.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuBar
            // 
            this.menuBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.solverToolStripMenuItem});
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new System.Drawing.Size(1688, 40);
            this.menuBar.TabIndex = 1;
            this.menuBar.Text = "menuStrip1";
            // 
            // solverToolStripMenuItem
            // 
            this.solverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.localDescentMenuItem,
            this.simulatedAnnealingMenuItem});
            this.solverToolStripMenuItem.Name = "solverToolStripMenuItem";
            this.solverToolStripMenuItem.Size = new System.Drawing.Size(132, 36);
            this.solverToolStripMenuItem.Text = "Optimizer";
            // 
            // localDescentMenuItem
            // 
            this.localDescentMenuItem.Name = "localDescentMenuItem";
            this.localDescentMenuItem.Size = new System.Drawing.Size(336, 38);
            this.localDescentMenuItem.Text = "Local Descent";
            this.localDescentMenuItem.Click += new System.EventHandler(this.localDescentMenuItem_Click);
            // 
            // simulatedAnnealingMenuItem
            // 
            this.simulatedAnnealingMenuItem.Name = "simulatedAnnealingMenuItem";
            this.simulatedAnnealingMenuItem.Size = new System.Drawing.Size(336, 38);
            this.simulatedAnnealingMenuItem.Text = "Simulated Annealing";
            this.simulatedAnnealingMenuItem.Click += new System.EventHandler(this.simulatedAnnealingMenuItem_Click);
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.algorithmStatus,
            this.costValueStatus});
            this.statusBar.Location = new System.Drawing.Point(0, 774);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1688, 38);
            this.statusBar.TabIndex = 2;
            // 
            // algorithmStatus
            // 
            this.algorithmStatus.Name = "algorithmStatus";
            this.algorithmStatus.Size = new System.Drawing.Size(79, 33);
            this.algorithmStatus.Text = "Status";
            // 
            // costValueStatus
            // 
            this.costValueStatus.Name = "costValueStatus";
            this.costValueStatus.Size = new System.Drawing.Size(62, 33);
            this.costValueStatus.Text = "Cost";
            // 
            // LocalSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1688, 812);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuBar);
            this.MainMenuStrip = this.menuBar;
            this.Name = "LocalSearchForm";
            this.Text = "LocalSearchOptimizationGUI";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.LocalSearchForm_Paint);
            this.Resize += new System.EventHandler(this.LocalSearchForm_Resize);
            this.menuBar.ResumeLayout(false);
            this.menuBar.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuBar;
        private System.Windows.Forms.ToolStripMenuItem solverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulatedAnnealingMenuItem;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel costValueStatus;
        private System.Windows.Forms.ToolStripStatusLabel algorithmStatus;
        private System.Windows.Forms.ToolStripMenuItem localDescentMenuItem;
    }
}

