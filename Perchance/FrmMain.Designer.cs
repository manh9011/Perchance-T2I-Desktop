
namespace Perchance
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            spcMain = new SplitContainer();
            splitContainer6 = new SplitContainer();
            splitContainer5 = new SplitContainer();
            txtDescription = new TextBox();
            config = new BindingSource(components);
            tsDescription = new ToolStrip();
            toolStripLabel9 = new ToolStripLabel();
            btnVietnamese = new ToolStripButton();
            btnEnglish = new ToolStripButton();
            txtNegative = new TextBox();
            tsNegatives = new ToolStrip();
            toolStripLabel10 = new ToolStripLabel();
            panHistory = new Panel();
            lvHistory = new ListView();
            ilHistory = new ImageList(components);
            tsHistory = new ToolStrip();
            btnRefreshHistory = new ToolStripButton();
            toolStripLabel11 = new ToolStripLabel();
            txtFilter = new ToolStripTextBox();
            btnOldFirst = new ToolStripButton();
            btnNewFirst = new ToolStripButton();
            tlpLayout = new TableLayoutPanel();
            toolbar = new ToolStrip();
            toolStripLabel2 = new ToolStripLabel();
            cboArtStyle = new ToolStripComboBox();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripLabel3 = new ToolStripLabel();
            txtWidth = new ToolStripTextBox();
            toolStripLabel4 = new ToolStripLabel();
            txtHeight = new ToolStripTextBox();
            btnPortrait = new ToolStripButton();
            btnLandscape = new ToolStripButton();
            btnSquare = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripLabel1 = new ToolStripLabel();
            btnClose = new ToolStripButton();
            tbrCount = new ToolStripTrackBar();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripLabel16 = new ToolStripLabel();
            tbrGuidanceScale = new ToolStripTrackBar();
            toolStripSeparator5 = new ToolStripSeparator();
            btnGenerate = new ToolStripButton();
            toolStripSeparator6 = new ToolStripSeparator();
            btnRandomGen = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            btnCancel = new ToolStripButton();
            tmrSearch = new System.Windows.Forms.Timer(components);
            cmsHistory = new ContextMenuStrip(components);
            mniOpenFolder = new ToolStripMenuItem();
            mniDelete = new ToolStripMenuItem();
            mniCopyPath = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            mniRefresh = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)spcMain).BeginInit();
            spcMain.Panel1.SuspendLayout();
            spcMain.Panel2.SuspendLayout();
            spcMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer6).BeginInit();
            splitContainer6.Panel1.SuspendLayout();
            splitContainer6.Panel2.SuspendLayout();
            splitContainer6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer5).BeginInit();
            splitContainer5.Panel1.SuspendLayout();
            splitContainer5.Panel2.SuspendLayout();
            splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)config).BeginInit();
            tsDescription.SuspendLayout();
            tsNegatives.SuspendLayout();
            panHistory.SuspendLayout();
            tsHistory.SuspendLayout();
            toolbar.SuspendLayout();
            cmsHistory.SuspendLayout();
            SuspendLayout();
            // 
            // spcMain
            // 
            spcMain.BackColor = SystemColors.ControlDark;
            spcMain.Dock = DockStyle.Fill;
            spcMain.FixedPanel = FixedPanel.Panel1;
            spcMain.Location = new Point(0, 25);
            spcMain.Name = "spcMain";
            // 
            // spcMain.Panel1
            // 
            spcMain.Panel1.Controls.Add(splitContainer6);
            spcMain.Panel1.Padding = new Padding(4, 4, 0, 4);
            // 
            // spcMain.Panel2
            // 
            spcMain.Panel2.AutoScroll = true;
            spcMain.Panel2.BackColor = SystemColors.ControlDark;
            spcMain.Panel2.Controls.Add(tlpLayout);
            spcMain.Panel2.Padding = new Padding(0, 4, 4, 4);
            spcMain.Size = new Size(1262, 416);
            spcMain.SplitterDistance = 300;
            spcMain.TabIndex = 0;
            // 
            // splitContainer6
            // 
            splitContainer6.Dock = DockStyle.Fill;
            splitContainer6.Location = new Point(4, 4);
            splitContainer6.Name = "splitContainer6";
            splitContainer6.Orientation = Orientation.Horizontal;
            // 
            // splitContainer6.Panel1
            // 
            splitContainer6.Panel1.Controls.Add(splitContainer5);
            // 
            // splitContainer6.Panel2
            // 
            splitContainer6.Panel2.Controls.Add(panHistory);
            splitContainer6.Size = new Size(296, 408);
            splitContainer6.SplitterDistance = 202;
            splitContainer6.TabIndex = 2;
            // 
            // splitContainer5
            // 
            splitContainer5.BorderStyle = BorderStyle.FixedSingle;
            splitContainer5.Dock = DockStyle.Fill;
            splitContainer5.Location = new Point(0, 0);
            splitContainer5.Name = "splitContainer5";
            splitContainer5.Orientation = Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            splitContainer5.Panel1.Controls.Add(txtDescription);
            splitContainer5.Panel1.Controls.Add(tsDescription);
            // 
            // splitContainer5.Panel2
            // 
            splitContainer5.Panel2.Controls.Add(txtNegative);
            splitContainer5.Panel2.Controls.Add(tsNegatives);
            splitContainer5.Size = new Size(296, 202);
            splitContainer5.SplitterDistance = 97;
            splitContainer5.TabIndex = 4;
            // 
            // txtDescription
            // 
            txtDescription.BackColor = Color.White;
            txtDescription.BorderStyle = BorderStyle.None;
            txtDescription.DataBindings.Add(new Binding("Text", config, "RawDescription", true));
            txtDescription.Dock = DockStyle.Fill;
            txtDescription.Location = new Point(0, 25);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.ScrollBars = ScrollBars.Vertical;
            txtDescription.Size = new Size(294, 70);
            txtDescription.TabIndex = 1;
            // 
            // config
            // 
            config.DataSource = typeof(Configuration);
            // 
            // tsDescription
            // 
            tsDescription.BackColor = Color.White;
            tsDescription.GripStyle = ToolStripGripStyle.Hidden;
            tsDescription.Items.AddRange(new ToolStripItem[] { toolStripLabel9, btnVietnamese, btnEnglish });
            tsDescription.Location = new Point(0, 0);
            tsDescription.Name = "tsDescription";
            tsDescription.Padding = new Padding(0);
            tsDescription.RenderMode = ToolStripRenderMode.System;
            tsDescription.Size = new Size(294, 25);
            tsDescription.TabIndex = 0;
            tsDescription.Text = "tsDescription";
            // 
            // toolStripLabel9
            // 
            toolStripLabel9.Name = "toolStripLabel9";
            toolStripLabel9.Size = new Size(67, 22);
            toolStripLabel9.Text = "Description";
            // 
            // btnVietnamese
            // 
            btnVietnamese.Alignment = ToolStripItemAlignment.Right;
            btnVietnamese.CheckOnClick = true;
            btnVietnamese.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnVietnamese.Image = (Image)resources.GetObject("btnVietnamese.Image");
            btnVietnamese.ImageTransparentColor = Color.Magenta;
            btnVietnamese.Name = "btnVietnamese";
            btnVietnamese.Size = new Size(23, 22);
            btnVietnamese.Text = "Vietnamese";
            btnVietnamese.CheckedChanged += btnVietnamese_CheckedChanged;
            // 
            // btnEnglish
            // 
            btnEnglish.Alignment = ToolStripItemAlignment.Right;
            btnEnglish.Checked = true;
            btnEnglish.CheckOnClick = true;
            btnEnglish.CheckState = CheckState.Checked;
            btnEnglish.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnEnglish.Image = (Image)resources.GetObject("btnEnglish.Image");
            btnEnglish.ImageTransparentColor = Color.Magenta;
            btnEnglish.Name = "btnEnglish";
            btnEnglish.Size = new Size(23, 22);
            btnEnglish.Text = "English";
            btnEnglish.CheckedChanged += btnEnglish_CheckedChanged;
            // 
            // txtNegative
            // 
            txtNegative.BackColor = Color.White;
            txtNegative.BorderStyle = BorderStyle.None;
            txtNegative.DataBindings.Add(new Binding("Text", config, "RawNegative", true));
            txtNegative.Dock = DockStyle.Fill;
            txtNegative.Location = new Point(0, 25);
            txtNegative.Multiline = true;
            txtNegative.Name = "txtNegative";
            txtNegative.ScrollBars = ScrollBars.Vertical;
            txtNegative.Size = new Size(294, 74);
            txtNegative.TabIndex = 1;
            // 
            // tsNegatives
            // 
            tsNegatives.BackColor = Color.White;
            tsNegatives.GripStyle = ToolStripGripStyle.Hidden;
            tsNegatives.Items.AddRange(new ToolStripItem[] { toolStripLabel10 });
            tsNegatives.Location = new Point(0, 0);
            tsNegatives.Name = "tsNegatives";
            tsNegatives.Padding = new Padding(0);
            tsNegatives.RenderMode = ToolStripRenderMode.System;
            tsNegatives.Size = new Size(294, 25);
            tsNegatives.TabIndex = 0;
            tsNegatives.Text = "tsNegatives";
            // 
            // toolStripLabel10
            // 
            toolStripLabel10.Name = "toolStripLabel10";
            toolStripLabel10.Size = new Size(54, 22);
            toolStripLabel10.Text = "Negative";
            // 
            // panHistory
            // 
            panHistory.BorderStyle = BorderStyle.FixedSingle;
            panHistory.Controls.Add(lvHistory);
            panHistory.Controls.Add(tsHistory);
            panHistory.Dock = DockStyle.Fill;
            panHistory.Location = new Point(0, 0);
            panHistory.Name = "panHistory";
            panHistory.Size = new Size(296, 202);
            panHistory.TabIndex = 0;
            // 
            // lvHistory
            // 
            lvHistory.BackColor = Color.White;
            lvHistory.BorderStyle = BorderStyle.None;
            lvHistory.Dock = DockStyle.Fill;
            lvHistory.FullRowSelect = true;
            lvHistory.GridLines = true;
            lvHistory.LargeImageList = ilHistory;
            lvHistory.Location = new Point(0, 25);
            lvHistory.Name = "lvHistory";
            lvHistory.Size = new Size(294, 175);
            lvHistory.TabIndex = 1;
            lvHistory.UseCompatibleStateImageBehavior = false;
            lvHistory.View = View.Tile;
            lvHistory.KeyUp += lvHistory_KeyUp;
            lvHistory.MouseDoubleClick += lvHistory_MouseDoubleClick;
            lvHistory.MouseUp += lvHistory_MouseUp;
            // 
            // ilHistory
            // 
            ilHistory.ColorDepth = ColorDepth.Depth32Bit;
            ilHistory.ImageStream = (ImageListStreamer)resources.GetObject("ilHistory.ImageStream");
            ilHistory.TransparentColor = Color.Transparent;
            ilHistory.Images.SetKeyName(0, "NO_IMAGE");
            // 
            // tsHistory
            // 
            tsHistory.BackColor = Color.White;
            tsHistory.GripStyle = ToolStripGripStyle.Hidden;
            tsHistory.Items.AddRange(new ToolStripItem[] { btnRefreshHistory, toolStripLabel11, txtFilter, btnOldFirst, btnNewFirst });
            tsHistory.Location = new Point(0, 0);
            tsHistory.Name = "tsHistory";
            tsHistory.Padding = new Padding(0);
            tsHistory.RenderMode = ToolStripRenderMode.System;
            tsHistory.Size = new Size(294, 25);
            tsHistory.TabIndex = 0;
            tsHistory.Text = "tsHistory";
            // 
            // btnRefreshHistory
            // 
            btnRefreshHistory.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnRefreshHistory.Image = (Image)resources.GetObject("btnRefreshHistory.Image");
            btnRefreshHistory.ImageTransparentColor = Color.Magenta;
            btnRefreshHistory.Name = "btnRefreshHistory";
            btnRefreshHistory.Size = new Size(23, 22);
            btnRefreshHistory.Text = "Refresh";
            btnRefreshHistory.Click += btnRefreshHistory_Click;
            // 
            // toolStripLabel11
            // 
            toolStripLabel11.Name = "toolStripLabel11";
            toolStripLabel11.Size = new Size(45, 22);
            toolStripLabel11.Text = "History";
            // 
            // txtFilter
            // 
            txtFilter.Alignment = ToolStripItemAlignment.Right;
            txtFilter.BorderStyle = BorderStyle.FixedSingle;
            txtFilter.Name = "txtFilter";
            txtFilter.Size = new Size(100, 25);
            txtFilter.TextChanged += txtFilter_TextChanged;
            // 
            // btnOldFirst
            // 
            btnOldFirst.Alignment = ToolStripItemAlignment.Right;
            btnOldFirst.CheckOnClick = true;
            btnOldFirst.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnOldFirst.Image = (Image)resources.GetObject("btnOldFirst.Image");
            btnOldFirst.ImageTransparentColor = Color.Magenta;
            btnOldFirst.Name = "btnOldFirst";
            btnOldFirst.Size = new Size(23, 22);
            btnOldFirst.Text = "toolStripButton2";
            btnOldFirst.CheckedChanged += btnOldFirst_CheckedChanged;
            // 
            // btnNewFirst
            // 
            btnNewFirst.Alignment = ToolStripItemAlignment.Right;
            btnNewFirst.Checked = true;
            btnNewFirst.CheckOnClick = true;
            btnNewFirst.CheckState = CheckState.Checked;
            btnNewFirst.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnNewFirst.Image = (Image)resources.GetObject("btnNewFirst.Image");
            btnNewFirst.ImageTransparentColor = Color.Magenta;
            btnNewFirst.Name = "btnNewFirst";
            btnNewFirst.Size = new Size(23, 22);
            btnNewFirst.Text = "toolStripButton3";
            btnNewFirst.CheckedChanged += btnNewFirst_CheckedChanged;
            // 
            // tlpLayout
            // 
            tlpLayout.AutoSize = true;
            tlpLayout.ColumnCount = 3;
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            tlpLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            tlpLayout.Dock = DockStyle.Top;
            tlpLayout.Location = new Point(0, 4);
            tlpLayout.Name = "tlpLayout";
            tlpLayout.RowCount = 11;
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.RowStyles.Add(new RowStyle());
            tlpLayout.Size = new Size(954, 0);
            tlpLayout.TabIndex = 0;
            // 
            // toolbar
            // 
            toolbar.AutoSize = false;
            toolbar.BackColor = Color.White;
            toolbar.GripStyle = ToolStripGripStyle.Hidden;
            toolbar.Items.AddRange(new ToolStripItem[] { toolStripLabel2, cboArtStyle, toolStripSeparator2, toolStripLabel3, txtWidth, toolStripLabel4, txtHeight, btnPortrait, btnLandscape, btnSquare, toolStripSeparator3, toolStripLabel1, btnClose, tbrCount, toolStripSeparator1, toolStripLabel16, tbrGuidanceScale, toolStripSeparator5, btnGenerate, toolStripSeparator6, btnRandomGen, toolStripSeparator4, btnCancel });
            toolbar.Location = new Point(0, 0);
            toolbar.Name = "toolbar";
            toolbar.Padding = new Padding(0);
            toolbar.RenderMode = ToolStripRenderMode.System;
            toolbar.Size = new Size(1262, 25);
            toolbar.TabIndex = 0;
            toolbar.Text = "toolStrip1";
            // 
            // toolStripLabel2
            // 
            toolStripLabel2.Name = "toolStripLabel2";
            toolStripLabel2.Size = new Size(51, 22);
            toolStripLabel2.Text = "Art Style";
            // 
            // cboArtStyle
            // 
            cboArtStyle.DataBindings.Add(new Binding("SelectedItem", config, "ArtStyle", true));
            cboArtStyle.DropDownStyle = ComboBoxStyle.DropDownList;
            cboArtStyle.FlatStyle = FlatStyle.Standard;
            cboArtStyle.Name = "cboArtStyle";
            cboArtStyle.Size = new Size(150, 25);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // toolStripLabel3
            // 
            toolStripLabel3.Name = "toolStripLabel3";
            toolStripLabel3.Size = new Size(39, 22);
            toolStripLabel3.Text = "Width";
            // 
            // txtWidth
            // 
            txtWidth.BorderStyle = BorderStyle.FixedSingle;
            txtWidth.DataBindings.Add(new Binding("Text", config, "Width", true));
            txtWidth.Name = "txtWidth";
            txtWidth.ReadOnly = true;
            txtWidth.Size = new Size(50, 25);
            // 
            // toolStripLabel4
            // 
            toolStripLabel4.Name = "toolStripLabel4";
            toolStripLabel4.Size = new Size(43, 22);
            toolStripLabel4.Text = "Height";
            // 
            // txtHeight
            // 
            txtHeight.BorderStyle = BorderStyle.FixedSingle;
            txtHeight.DataBindings.Add(new Binding("Text", config, "Height", true));
            txtHeight.Name = "txtHeight";
            txtHeight.ReadOnly = true;
            txtHeight.Size = new Size(50, 25);
            // 
            // btnPortrait
            // 
            btnPortrait.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnPortrait.Image = (Image)resources.GetObject("btnPortrait.Image");
            btnPortrait.ImageTransparentColor = Color.Magenta;
            btnPortrait.Name = "btnPortrait";
            btnPortrait.Size = new Size(23, 22);
            btnPortrait.Text = "Portrait";
            btnPortrait.Click += btnPortrait_Click;
            // 
            // btnLandscape
            // 
            btnLandscape.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnLandscape.Image = (Image)resources.GetObject("btnLandscape.Image");
            btnLandscape.ImageTransparentColor = Color.Magenta;
            btnLandscape.Name = "btnLandscape";
            btnLandscape.Size = new Size(23, 22);
            btnLandscape.Text = "Landscape";
            btnLandscape.Click += btnLandscape_Click;
            // 
            // btnSquare
            // 
            btnSquare.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnSquare.Image = (Image)resources.GetObject("btnSquare.Image");
            btnSquare.ImageTransparentColor = Color.Magenta;
            btnSquare.Name = "btnSquare";
            btnSquare.Size = new Size(23, 22);
            btnSquare.Text = "Square";
            btnSquare.Click += btnSquare_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(40, 22);
            toolStripLabel1.Text = "Count";
            // 
            // btnClose
            // 
            btnClose.Alignment = ToolStripItemAlignment.Right;
            btnClose.Image = (Image)resources.GetObject("btnClose.Image");
            btnClose.ImageTransparentColor = Color.Magenta;
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(56, 22);
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;
            // 
            // tbrCount
            // 
            tbrCount.DataBindings.Add(new Binding("Value", config, "Count", true));
            tbrCount.Maximum = 30;
            tbrCount.Minimum = 1;
            tbrCount.Name = "tbrCount";
            tbrCount.RightToLeftLayout = true;
            tbrCount.Size = new Size(150, 22);
            tbrCount.Value = 3;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // toolStripLabel16
            // 
            toolStripLabel16.Name = "toolStripLabel16";
            toolStripLabel16.Size = new Size(87, 22);
            toolStripLabel16.Text = "Guidance Scale";
            // 
            // tbrGuidanceScale
            // 
            tbrGuidanceScale.DataBindings.Add(new Binding("Value", config, "GuidanceScale", true));
            tbrGuidanceScale.Maximum = 30;
            tbrGuidanceScale.Minimum = 1;
            tbrGuidanceScale.Name = "tbrGuidanceScale";
            tbrGuidanceScale.Size = new Size(150, 22);
            tbrGuidanceScale.Value = 7;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 25);
            // 
            // btnGenerate
            // 
            btnGenerate.Image = (Image)resources.GetObject("btnGenerate.Image");
            btnGenerate.ImageTransparentColor = Color.Magenta;
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(74, 22);
            btnGenerate.Text = "Generate";
            btnGenerate.Click += btnGenerate_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 25);
            // 
            // btnRandomGen
            // 
            btnRandomGen.Image = (Image)resources.GetObject("btnRandomGen.Image");
            btnRandomGen.ImageTransparentColor = Color.Magenta;
            btnRandomGen.Name = "btnRandomGen";
            btnRandomGen.Size = new Size(96, 22);
            btnRandomGen.Text = "Random Gen";
            btnRandomGen.Click += btnRandomGen_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // btnCancel
            // 
            btnCancel.Image = (Image)resources.GetObject("btnCancel.Image");
            btnCancel.ImageTransparentColor = Color.Magenta;
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(63, 22);
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // tmrSearch
            // 
            tmrSearch.Interval = 1000;
            tmrSearch.Tick += tmrSearch_Tick;
            // 
            // cmsHistory
            // 
            cmsHistory.Items.AddRange(new ToolStripItem[] { mniOpenFolder, mniDelete, mniCopyPath, toolStripMenuItem1, mniRefresh });
            cmsHistory.Name = "cmsImage";
            cmsHistory.Size = new Size(140, 98);
            // 
            // mniOpenFolder
            // 
            mniOpenFolder.Image = (Image)resources.GetObject("mniOpenFolder.Image");
            mniOpenFolder.Name = "mniOpenFolder";
            mniOpenFolder.Size = new Size(139, 22);
            mniOpenFolder.Text = "Open Folder";
            mniOpenFolder.Click += mniOpenFolder_Click;
            // 
            // mniDelete
            // 
            mniDelete.Image = (Image)resources.GetObject("mniDelete.Image");
            mniDelete.Name = "mniDelete";
            mniDelete.Size = new Size(139, 22);
            mniDelete.Text = "Delete";
            mniDelete.Click += mniDelete_Click;
            // 
            // mniCopyPath
            // 
            mniCopyPath.Image = (Image)resources.GetObject("mniCopyPath.Image");
            mniCopyPath.Name = "mniCopyPath";
            mniCopyPath.Size = new Size(139, 22);
            mniCopyPath.Text = "Copy Path";
            mniCopyPath.Click += mniCopyPath_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(136, 6);
            // 
            // mniRefresh
            // 
            mniRefresh.Image = (Image)resources.GetObject("mniRefresh.Image");
            mniRefresh.Name = "mniRefresh";
            mniRefresh.Size = new Size(139, 22);
            mniRefresh.Text = "Refresh";
            mniRefresh.Click += mniRefresh_Click;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDark;
            ClientSize = new Size(1262, 441);
            Controls.Add(spcMain);
            Controls.Add(toolbar);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Perchance Text To Image";
            WindowState = FormWindowState.Maximized;
            Load += FrmMain_Load;
            spcMain.Panel1.ResumeLayout(false);
            spcMain.Panel2.ResumeLayout(false);
            spcMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)spcMain).EndInit();
            spcMain.ResumeLayout(false);
            splitContainer6.Panel1.ResumeLayout(false);
            splitContainer6.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer6).EndInit();
            splitContainer6.ResumeLayout(false);
            splitContainer5.Panel1.ResumeLayout(false);
            splitContainer5.Panel1.PerformLayout();
            splitContainer5.Panel2.ResumeLayout(false);
            splitContainer5.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer5).EndInit();
            splitContainer5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)config).EndInit();
            tsDescription.ResumeLayout(false);
            tsDescription.PerformLayout();
            tsNegatives.ResumeLayout(false);
            tsNegatives.PerformLayout();
            panHistory.ResumeLayout(false);
            panHistory.PerformLayout();
            tsHistory.ResumeLayout(false);
            tsHistory.PerformLayout();
            toolbar.ResumeLayout(false);
            toolbar.PerformLayout();
            cmsHistory.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer spcMain;
        private TextBox txtDescription;
        private TextBox txtNegative;
        private ToolStrip toolbar;
        private ToolStripButton btnGenerate;
        private ToolStripLabel toolStripLabel2;
        private ToolStripComboBox cboArtStyle;
        private ToolStripLabel toolStripLabel3;
        private ToolStripTextBox txtWidth;
        private ToolStripLabel toolStripLabel4;
        private ToolStripTextBox txtHeight;
        private ToolStripLabel toolStripLabel1;
        private ToolStripButton btnClose;
        private SplitContainer splitContainer5;
        private BindingSource config;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStrip tsDescription;
        private ToolStripLabel toolStripLabel9;
        private ToolStrip tsNegatives;
        private ToolStripLabel toolStripLabel10;
        private ToolStripButton btnRandomGen;
        private ToolStripButton btnVietnamese;
        private ToolStripButton btnEnglish;
        private ToolStripButton btnPortrait;
        private ToolStripButton btnLandscape;
        private ToolStripButton btnSquare;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private SplitContainer splitContainer6;
        private Panel panHistory;
        private ToolStrip tsHistory;
        private ToolStripLabel toolStripLabel11;
        private ListView lvHistory;
        private ToolStripTextBox txtFilter;
        private ToolStripButton btnRefreshHistory;
        private ToolStripButton btnOldFirst;
        private ToolStripButton btnNewFirst;
        private System.Windows.Forms.Timer tmrSearch;
        private ImageList ilHistory;
        private ContextMenuStrip cmsHistory;
        private ToolStripMenuItem mniOpenFolder;
        private ToolStripMenuItem mniDelete;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mniRefresh;
        private ToolStripLabel toolStripLabel16;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem mniCopyPath;
        private TableLayoutPanel tlpLayout;
        private ToolStripTrackBar tbrCount;
        private ToolStripTrackBar tbrGuidanceScale;
        private ToolStripButton btnCancel;
        private ToolStripSeparator toolStripSeparator6;
    }
}
