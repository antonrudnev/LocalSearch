namespace TestForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.solverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.localDescentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulatedAnnealingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dimensionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dimesionTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.multistartOptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multistartTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.displayCostMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.algorithmStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.costValueStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.iterationStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.solverToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1688, 40);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // solverToolStripMenuItem
            // 
            this.solverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.localDescentMenuItem,
            this.simulatedAnnealingMenuItem});
            this.solverToolStripMenuItem.Name = "solverToolStripMenuItem";
            this.solverToolStripMenuItem.Size = new System.Drawing.Size(93, 36);
            this.solverToolStripMenuItem.Text = "Solver";
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
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dimensionMenuItem,
            this.multistartOptionToolStripMenuItem,
            this.displayCostMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(111, 36);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // dimensionMenuItem
            // 
            this.dimensionMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dimesionTextBox});
            this.dimensionMenuItem.Name = "dimensionMenuItem";
            this.dimensionMenuItem.Size = new System.Drawing.Size(297, 38);
            this.dimensionMenuItem.Text = "Dimension";
            // 
            // dimesionTextBox
            // 
            this.dimesionTextBox.Name = "dimesionTextBox";
            this.dimesionTextBox.Size = new System.Drawing.Size(100, 39);
            this.dimesionTextBox.Text = "50";
            this.dimesionTextBox.TextChanged += new System.EventHandler(this.dimesionTextBox_TextChanged);
            // 
            // multistartOptionToolStripMenuItem
            // 
            this.multistartOptionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.multistartTextBox});
            this.multistartOptionToolStripMenuItem.Name = "multistartOptionToolStripMenuItem";
            this.multistartOptionToolStripMenuItem.Size = new System.Drawing.Size(297, 38);
            this.multistartOptionToolStripMenuItem.Text = "Multistart Option";
            // 
            // multistartTextBox
            // 
            this.multistartTextBox.Name = "multistartTextBox";
            this.multistartTextBox.Size = new System.Drawing.Size(100, 39);
            this.multistartTextBox.Text = "1";
            this.multistartTextBox.TextChanged += new System.EventHandler(this.multistartTextBox_TextChanged);
            // 
            // displayCostMenuItem
            // 
            this.displayCostMenuItem.CheckOnClick = true;
            this.displayCostMenuItem.Name = "displayCostMenuItem";
            this.displayCostMenuItem.Size = new System.Drawing.Size(297, 38);
            this.displayCostMenuItem.Text = "Display Cost";
            this.displayCostMenuItem.CheckedChanged += new System.EventHandler(this.displayCostMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.algorithmStatus,
            this.costValueStatus,
            this.iterationStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 775);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1688, 37);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // algorithmStatus
            // 
            this.algorithmStatus.Name = "algorithmStatus";
            this.algorithmStatus.Size = new System.Drawing.Size(79, 32);
            this.algorithmStatus.Text = "Status";
            // 
            // costValueStatus
            // 
            this.costValueStatus.Name = "costValueStatus";
            this.costValueStatus.Size = new System.Drawing.Size(62, 32);
            this.costValueStatus.Text = "Cost";
            // 
            // iterationStatus
            // 
            this.iterationStatus.Name = "iterationStatus";
            this.iterationStatus.Size = new System.Drawing.Size(104, 32);
            this.iterationStatus.Text = "Iteration";
            // 
            // LocalSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1688, 812);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "LocalSearchForm";
            this.Text = "LocalSearch";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.SizeChanged += new System.EventHandler(this.LocalSearchForm_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.LocalSearchForm_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LocalSearchForm_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem solverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simulatedAnnealingMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel costValueStatus;
        private System.Windows.Forms.ToolStripStatusLabel algorithmStatus;
        private System.Windows.Forms.ToolStripMenuItem localDescentMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel iterationStatus;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayCostMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multistartOptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox multistartTextBox;
        private System.Windows.Forms.ToolStripMenuItem dimensionMenuItem;
        private System.Windows.Forms.ToolStripTextBox dimesionTextBox;
    }
}

