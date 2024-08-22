namespace Perchance
{
    partial class Toast
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
            components = new System.ComponentModel.Container();
            lblDescription = new Label();
            lblCaption = new Label();
            tmrAnimIn = new System.Windows.Forms.Timer(components);
            tmrAnimOut = new System.Windows.Forms.Timer(components);
            tmrTimeline = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // lblDescription
            // 
            lblDescription.BackColor = Color.DimGray;
            lblDescription.BorderStyle = BorderStyle.FixedSingle;
            lblDescription.Dock = DockStyle.Fill;
            lblDescription.ForeColor = Color.White;
            lblDescription.Location = new Point(0, 23);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(284, 58);
            lblDescription.TabIndex = 6;
            lblDescription.Text = "Information for what !";
            lblDescription.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblCaption
            // 
            lblCaption.BackColor = Color.Gray;
            lblCaption.BorderStyle = BorderStyle.FixedSingle;
            lblCaption.Dock = DockStyle.Top;
            lblCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCaption.ForeColor = Color.White;
            lblCaption.Location = new Point(0, 0);
            lblCaption.Name = "lblCaption";
            lblCaption.Size = new Size(284, 23);
            lblCaption.TabIndex = 7;
            lblCaption.Text = "ℹ️ Information";
            lblCaption.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tmrAnimIn
            // 
            tmrAnimIn.Tick += tmrAnimIn_Tick;
            // 
            // tmrAnimOut
            // 
            tmrAnimOut.Tick += tmrAnimOut_Tick;
            // 
            // tmrTimeline
            // 
            tmrTimeline.Tick += tmrTimeline_Tick;
            // 
            // FrmToast
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(284, 81);
            Controls.Add(lblDescription);
            Controls.Add(lblCaption);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmToast";
            ShowIcon = false;
            StartPosition = FormStartPosition.Manual;
            Text = "FrmToast";
            Shown += FrmToast_Shown;
            ResumeLayout(false);
        }

        #endregion

        private Label lblDescription;
        private Label lblCaption;
        private System.Windows.Forms.Timer tmrAnimIn;
        private System.Windows.Forms.Timer tmrAnimOut;
        private System.Windows.Forms.Timer tmrTimeline;
    }
}