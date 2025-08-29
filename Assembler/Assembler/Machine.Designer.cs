namespace Assembler
{
    partial class Machine
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Machine));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.WordCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.MainMemoryTextLabel = new System.Windows.Forms.Label();
            this.RegisterTextLabel = new System.Windows.Forms.Label();
            this.SourceCodeTextLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.ErrorMenuTextBox = new System.Windows.Forms.RichTextBox();
            this.ErrorMenuTextLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.UserInputBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.MachineCodeTextBox = new System.Windows.Forms.TextBox();
            this.MainMemoryPanel = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.RegistersPanel = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveASToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fAQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assembleCOdeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.executionSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleDenaryModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMachineCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lMCModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearRegistersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.fAQToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadExamplesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.instructionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.SpeedOfExecution_txt = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ConfirmSpeed = new System.Windows.Forms.Button();
            this.LabelOfSpeed = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserInputBox)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.WordCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 727);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1370, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // WordCount
            // 
            this.WordCount.Name = "WordCount";
            this.WordCount.Size = new System.Drawing.Size(78, 17);
            this.WordCount.Text = "Word Count: ";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.81752F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.09489F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.08759F));
            this.tableLayoutPanel1.Controls.Add(this.MainMemoryTextLabel, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.RegisterTextLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.SourceCodeTextLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.MainMemoryPanel, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.toolStrip2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.534851F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.75818F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1.628567F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1370, 703);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // MainMemoryTextLabel
            // 
            this.MainMemoryTextLabel.AutoSize = true;
            this.MainMemoryTextLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainMemoryTextLabel.Font = new System.Drawing.Font("Palatino Linotype", 8.25F, System.Drawing.FontStyle.Bold);
            this.MainMemoryTextLabel.Location = new System.Drawing.Point(769, 0);
            this.MainMemoryTextLabel.Name = "MainMemoryTextLabel";
            this.MainMemoryTextLabel.Size = new System.Drawing.Size(598, 60);
            this.MainMemoryTextLabel.TabIndex = 4;
            this.MainMemoryTextLabel.Text = "Main Memory";
            this.MainMemoryTextLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // RegisterTextLabel
            // 
            this.RegisterTextLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RegisterTextLabel.Font = new System.Drawing.Font("Palatino Linotype", 8.25F, System.Drawing.FontStyle.Bold);
            this.RegisterTextLabel.Location = new System.Drawing.Point(480, 0);
            this.RegisterTextLabel.Name = "RegisterTextLabel";
            this.RegisterTextLabel.Size = new System.Drawing.Size(283, 60);
            this.RegisterTextLabel.TabIndex = 3;
            this.RegisterTextLabel.Text = "Registers";
            this.RegisterTextLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // SourceCodeTextLabel
            // 
            this.SourceCodeTextLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SourceCodeTextLabel.Font = new System.Drawing.Font("Palatino Linotype", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SourceCodeTextLabel.Location = new System.Drawing.Point(3, 0);
            this.SourceCodeTextLabel.Name = "SourceCodeTextLabel";
            this.SourceCodeTextLabel.Size = new System.Drawing.Size(471, 60);
            this.SourceCodeTextLabel.TabIndex = 0;
            this.SourceCodeTextLabel.Text = "Source Code";
            this.SourceCodeTextLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Font = new System.Drawing.Font("Palatino Linotype", 8.25F, System.Drawing.FontStyle.Bold);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 63);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71.42857F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.57143F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(471, 625);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 449);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(465, 173);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.ErrorMenuTextBox, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.ErrorMenuTextLabel, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.28037F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.71963F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(465, 173);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // ErrorMenuTextBox
            // 
            this.ErrorMenuTextBox.BackColor = System.Drawing.Color.White;
            this.ErrorMenuTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorMenuTextBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorMenuTextBox.Location = new System.Drawing.Point(3, 20);
            this.ErrorMenuTextBox.Name = "ErrorMenuTextBox";
            this.ErrorMenuTextBox.ReadOnly = true;
            this.ErrorMenuTextBox.Size = new System.Drawing.Size(459, 150);
            this.ErrorMenuTextBox.TabIndex = 0;
            this.ErrorMenuTextBox.Text = "";
            // 
            // ErrorMenuTextLabel
            // 
            this.ErrorMenuTextLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorMenuTextLabel.Location = new System.Drawing.Point(3, 0);
            this.ErrorMenuTextLabel.Name = "ErrorMenuTextLabel";
            this.ErrorMenuTextLabel.Size = new System.Drawing.Size(459, 17);
            this.ErrorMenuTextLabel.TabIndex = 1;
            this.ErrorMenuTextLabel.Text = "Error Menu";
            this.ErrorMenuTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.47312F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.52688F));
            this.tableLayoutPanel3.Controls.Add(this.UserInputBox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.MachineCodeTextBox, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(465, 440);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // UserInputBox
            // 
            this.UserInputBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.UserInputBox.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*" +
    "(?<range>:)\\s*(?<range>[^;]+);";
            this.UserInputBox.AutoScrollMinSize = new System.Drawing.Size(35, 22);
            this.UserInputBox.BackBrush = null;
            this.UserInputBox.CharHeight = 22;
            this.UserInputBox.CharWidth = 12;
            this.UserInputBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.UserInputBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.UserInputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UserInputBox.Font = new System.Drawing.Font("Courier New", 15F, System.Drawing.FontStyle.Bold);
            this.UserInputBox.IsReplaceMode = false;
            this.UserInputBox.LineNumberColor = System.Drawing.Color.Black;
            this.UserInputBox.LineNumberStartValue = ((uint)(0u));
            this.UserInputBox.Location = new System.Drawing.Point(3, 3);
            this.UserInputBox.Name = "UserInputBox";
            this.UserInputBox.Paddings = new System.Windows.Forms.Padding(0);
            this.UserInputBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.UserInputBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("UserInputBox.ServiceColors")));
            this.UserInputBox.Size = new System.Drawing.Size(238, 434);
            this.UserInputBox.TabIndex = 0;
            this.UserInputBox.Zoom = 100;
            this.UserInputBox.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.UserInputBox_TextChanged);
            this.UserInputBox.Load += new System.EventHandler(this.UserInputBox_Load);
            // 
            // MachineCodeTextBox
            // 
            this.MachineCodeTextBox.BackColor = System.Drawing.Color.White;
            this.MachineCodeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MachineCodeTextBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MachineCodeTextBox.Location = new System.Drawing.Point(247, 3);
            this.MachineCodeTextBox.Multiline = true;
            this.MachineCodeTextBox.Name = "MachineCodeTextBox";
            this.MachineCodeTextBox.ReadOnly = true;
            this.MachineCodeTextBox.Size = new System.Drawing.Size(215, 434);
            this.MachineCodeTextBox.TabIndex = 2;
            // 
            // MainMemoryPanel
            // 
            this.MainMemoryPanel.AutoScroll = true;
            this.MainMemoryPanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.MainMemoryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainMemoryPanel.Location = new System.Drawing.Point(769, 63);
            this.MainMemoryPanel.Name = "MainMemoryPanel";
            this.MainMemoryPanel.Size = new System.Drawing.Size(598, 625);
            this.MainMemoryPanel.TabIndex = 2;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Location = new System.Drawing.Point(0, 691);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(477, 12);
            this.toolStrip2.TabIndex = 5;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoScroll = true;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.RegistersPanel, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(480, 63);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(283, 625);
            this.tableLayoutPanel5.TabIndex = 6;
            // 
            // RegistersPanel
            // 
            this.RegistersPanel.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.RegistersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RegistersPanel.Location = new System.Drawing.Point(3, 3);
            this.RegistersPanel.Name = "RegistersPanel";
            this.RegistersPanel.Size = new System.Drawing.Size(277, 619);
            this.RegistersPanel.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem1,
            this.helpToolStripMenuItem,
            this.helpToolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1370, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveASToolStripMenuItem1,
            this.openToolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveASToolStripMenuItem1
            // 
            this.saveASToolStripMenuItem1.Name = "saveASToolStripMenuItem1";
            this.saveASToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
            this.saveASToolStripMenuItem1.Text = "Save As";
            this.saveASToolStripMenuItem1.Click += new System.EventHandler(this.saveASToolStripMenuItem1_Click);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(114, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click_1);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fAQToolStripMenuItem,
            this.checkErrorsToolStripMenuItem,
            this.assembleCOdeToolStripMenuItem,
            this.executionSpeedToolStripMenuItem});
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(45, 20);
            this.helpToolStripMenuItem1.Text = "Extra";
            // 
            // fAQToolStripMenuItem
            // 
            this.fAQToolStripMenuItem.Name = "fAQToolStripMenuItem";
            this.fAQToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.fAQToolStripMenuItem.Text = "Load Into Memory";
            this.fAQToolStripMenuItem.Click += new System.EventHandler(this.fAQToolStripMenuItem_Click);
            // 
            // checkErrorsToolStripMenuItem
            // 
            this.checkErrorsToolStripMenuItem.Name = "checkErrorsToolStripMenuItem";
            this.checkErrorsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.checkErrorsToolStripMenuItem.Text = "Check Errors";
            this.checkErrorsToolStripMenuItem.Click += new System.EventHandler(this.checkErrorsToolStripMenuItem_Click_1);
            // 
            // assembleCOdeToolStripMenuItem
            // 
            this.assembleCOdeToolStripMenuItem.Name = "assembleCOdeToolStripMenuItem";
            this.assembleCOdeToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.assembleCOdeToolStripMenuItem.Text = "Assemble Code";
            this.assembleCOdeToolStripMenuItem.Click += new System.EventHandler(this.assembleCOdeToolStripMenuItem_Click);
            // 
            // executionSpeedToolStripMenuItem
            // 
            this.executionSpeedToolStripMenuItem.Name = "executionSpeedToolStripMenuItem";
            this.executionSpeedToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.executionSpeedToolStripMenuItem.Text = "Execution speed";
            this.executionSpeedToolStripMenuItem.Click += new System.EventHandler(this.executionSpeedToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleDenaryModeToolStripMenuItem,
            this.viewMachineCodeToolStripMenuItem,
            this.lMCModeToolStripMenuItem,
            this.clearRegistersToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "View";
            // 
            // toggleDenaryModeToolStripMenuItem
            // 
            this.toggleDenaryModeToolStripMenuItem.Name = "toggleDenaryModeToolStripMenuItem";
            this.toggleDenaryModeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.toggleDenaryModeToolStripMenuItem.Text = "Toggle Denary Mode";
            this.toggleDenaryModeToolStripMenuItem.Click += new System.EventHandler(this.toggleDenaryModeToolStripMenuItem_Click);
            // 
            // viewMachineCodeToolStripMenuItem
            // 
            this.viewMachineCodeToolStripMenuItem.Name = "viewMachineCodeToolStripMenuItem";
            this.viewMachineCodeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.viewMachineCodeToolStripMenuItem.Text = "View Machine Code";
            this.viewMachineCodeToolStripMenuItem.Click += new System.EventHandler(this.viewMachineCodeToolStripMenuItem_Click);
            // 
            // lMCModeToolStripMenuItem
            // 
            this.lMCModeToolStripMenuItem.Name = "lMCModeToolStripMenuItem";
            this.lMCModeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.lMCModeToolStripMenuItem.Text = "Clear Memory";
            this.lMCModeToolStripMenuItem.Click += new System.EventHandler(this.lMCModeToolStripMenuItem_Click);
            // 
            // clearRegistersToolStripMenuItem
            // 
            this.clearRegistersToolStripMenuItem.Name = "clearRegistersToolStripMenuItem";
            this.clearRegistersToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.clearRegistersToolStripMenuItem.Text = "Clear Registers";
            this.clearRegistersToolStripMenuItem.Click += new System.EventHandler(this.clearRegistersToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem2
            // 
            this.helpToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fAQToolStripMenuItem1,
            this.loadExamplesToolStripMenuItem1,
            this.instructionsToolStripMenuItem});
            this.helpToolStripMenuItem2.Name = "helpToolStripMenuItem2";
            this.helpToolStripMenuItem2.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem2.Text = "Help";
            // 
            // fAQToolStripMenuItem1
            // 
            this.fAQToolStripMenuItem1.Name = "fAQToolStripMenuItem1";
            this.fAQToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.fAQToolStripMenuItem1.Text = "FAQ";
            this.fAQToolStripMenuItem1.Click += new System.EventHandler(this.fAQToolStripMenuItem1_Click);
            // 
            // loadExamplesToolStripMenuItem1
            // 
            this.loadExamplesToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
            this.loadExamplesToolStripMenuItem1.Name = "loadExamplesToolStripMenuItem1";
            this.loadExamplesToolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.loadExamplesToolStripMenuItem1.Text = "Load Examples";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(192, 22);
            this.toolStripMenuItem2.Text = "Add To 5";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(192, 22);
            this.toolStripMenuItem3.Text = "Adding From Memory";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // instructionsToolStripMenuItem
            // 
            this.instructionsToolStripMenuItem.Name = "instructionsToolStripMenuItem";
            this.instructionsToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.instructionsToolStripMenuItem.Text = "Instructions";
            this.instructionsToolStripMenuItem.Click += new System.EventHandler(this.instructionsToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton3,
            this.toolStripButton1,
            this.toolStripButton6,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1370, 39);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton3.Text = "Run";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click_2);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton1.Text = "Stop";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton6.Text = "Step Run";
            this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton2.Text = "Pause/Play";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // SpeedOfExecution_txt
            // 
            this.SpeedOfExecution_txt.Location = new System.Drawing.Point(677, 384);
            this.SpeedOfExecution_txt.Multiline = true;
            this.SpeedOfExecution_txt.Name = "SpeedOfExecution_txt";
            this.SpeedOfExecution_txt.Size = new System.Drawing.Size(59, 22);
            this.SpeedOfExecution_txt.TabIndex = 9;
            this.SpeedOfExecution_txt.Text = " ";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(622, 384);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(49, 22);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "Speed: ";
            // 
            // ConfirmSpeed
            // 
            this.ConfirmSpeed.Location = new System.Drawing.Point(605, 431);
            this.ConfirmSpeed.Name = "ConfirmSpeed";
            this.ConfirmSpeed.Size = new System.Drawing.Size(178, 29);
            this.ConfirmSpeed.TabIndex = 7;
            this.ConfirmSpeed.Text = "Confirm";
            this.ConfirmSpeed.UseVisualStyleBackColor = true;
            this.ConfirmSpeed.Click += new System.EventHandler(this.ConfirmSpeed_Click);
            // 
            // LabelOfSpeed
            // 
            this.LabelOfSpeed.Location = new System.Drawing.Point(496, 288);
            this.LabelOfSpeed.Name = "LabelOfSpeed";
            this.LabelOfSpeed.Size = new System.Drawing.Size(379, 48);
            this.LabelOfSpeed.TabIndex = 6;
            this.LabelOfSpeed.Text = "Enter the speed of the program in milliseconds";
            this.LabelOfSpeed.UseVisualStyleBackColor = true;
            // 
            // Machine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 749);
            this.Controls.Add(this.SpeedOfExecution_txt);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.ConfirmSpeed);
            this.Controls.Add(this.LabelOfSpeed);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Machine";
            this.Text = "Assembler";
            this.Load += new System.EventHandler(this.Machine_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserInputBox)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel WordCount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.RichTextBox ErrorMenuTextBox;
        private System.Windows.Forms.Label ErrorMenuTextLabel;
        private System.Windows.Forms.Panel RegistersPanel;
        private System.Windows.Forms.Panel MainMemoryPanel;
        private System.Windows.Forms.Label MainMemoryTextLabel;
        private System.Windows.Forms.Label RegisterTextLabel;
        private System.Windows.Forms.Label SourceCodeTextLabel;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveASToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleDenaryModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMachineCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lMCModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fAQToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem fAQToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadExamplesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem checkErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.TextBox MachineCodeTextBox;
        private System.Windows.Forms.ToolStripMenuItem assembleCOdeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearRegistersToolStripMenuItem;
        private FastColoredTextBoxNS.FastColoredTextBox UserInputBox;
        private System.Windows.Forms.ToolStripMenuItem instructionsToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ToolStripMenuItem executionSpeedToolStripMenuItem;
        private System.Windows.Forms.TextBox SpeedOfExecution_txt;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button ConfirmSpeed;
        private System.Windows.Forms.Button LabelOfSpeed;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
    }
}

