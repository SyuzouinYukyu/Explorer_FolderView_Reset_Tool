namespace Explorer_FolderView_Reset_Tool;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private Label descriptionLabel;
    private GroupBox settingsGroup;
    private Label bagMruSizeLabel;
    private NumericUpDown bagMruSizeNumeric;
    private Label backupRootLabel;
    private TextBox backupRootTextBox;
    private Button browseBackupButton;
    private CheckBox restartExplorerCheckBox;
    private CheckBox openBackupAfterCompletionCheckBox;
    private CheckBox verboseLogCheckBox;
    private FlowLayoutPanel buttonPanel;
    private Button precheckButton;
    private Button backupOnlyButton;
    private Button repairButton;
    private Button restartExplorerButton;
    private Button openBackupButton;
    private Button restoreButton;
    private Button saveLogButton;
    private Button exitButton;
    private Label statusLabel;
    private TextBox logTextBox;
    private ContextMenuStrip logContextMenu;
    private ToolStripMenuItem selectAllMenuItem;
    private ToolStripMenuItem copyMenuItem;
    private ToolStripMenuItem clearMenuItem;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        descriptionLabel = new Label();
        settingsGroup = new GroupBox();
        bagMruSizeLabel = new Label();
        bagMruSizeNumeric = new NumericUpDown();
        backupRootLabel = new Label();
        backupRootTextBox = new TextBox();
        browseBackupButton = new Button();
        restartExplorerCheckBox = new CheckBox();
        openBackupAfterCompletionCheckBox = new CheckBox();
        verboseLogCheckBox = new CheckBox();
        buttonPanel = new FlowLayoutPanel();
        precheckButton = new Button();
        backupOnlyButton = new Button();
        repairButton = new Button();
        restartExplorerButton = new Button();
        openBackupButton = new Button();
        restoreButton = new Button();
        saveLogButton = new Button();
        exitButton = new Button();
        statusLabel = new Label();
        logTextBox = new TextBox();
        logContextMenu = new ContextMenuStrip(components);
        selectAllMenuItem = new ToolStripMenuItem();
        copyMenuItem = new ToolStripMenuItem();
        clearMenuItem = new ToolStripMenuItem();
        settingsGroup.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)bagMruSizeNumeric).BeginInit();
        buttonPanel.SuspendLayout();
        logContextMenu.SuspendLayout();
        SuspendLayout();
        // 
        // descriptionLabel
        // 
        descriptionLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        descriptionLabel.BackColor = SystemColors.Info;
        descriptionLabel.BorderStyle = BorderStyle.FixedSingle;
        descriptionLabel.Location = new Point(12, 12);
        descriptionLabel.Name = "descriptionLabel";
        descriptionLabel.Padding = new Padding(10, 8, 10, 8);
        descriptionLabel.Size = new Size(960, 54);
        descriptionLabel.TabIndex = 0;
        descriptionLabel.Text = "Explorerがフォルダごとの表示形式を記憶しない場合に、Bags / BagMRUをバックアップ後にリセットします。";
        descriptionLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // settingsGroup
        // 
        settingsGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        settingsGroup.Controls.Add(bagMruSizeLabel);
        settingsGroup.Controls.Add(bagMruSizeNumeric);
        settingsGroup.Controls.Add(backupRootLabel);
        settingsGroup.Controls.Add(backupRootTextBox);
        settingsGroup.Controls.Add(browseBackupButton);
        settingsGroup.Controls.Add(restartExplorerCheckBox);
        settingsGroup.Controls.Add(openBackupAfterCompletionCheckBox);
        settingsGroup.Controls.Add(verboseLogCheckBox);
        settingsGroup.Location = new Point(12, 78);
        settingsGroup.Name = "settingsGroup";
        settingsGroup.Size = new Size(960, 136);
        settingsGroup.TabIndex = 1;
        settingsGroup.TabStop = false;
        settingsGroup.Text = "設定";
        // 
        // bagMruSizeLabel
        // 
        bagMruSizeLabel.AutoSize = true;
        bagMruSizeLabel.Location = new Point(14, 31);
        bagMruSizeLabel.Name = "bagMruSizeLabel";
        bagMruSizeLabel.Size = new Size(100, 15);
        bagMruSizeLabel.TabIndex = 0;
        bagMruSizeLabel.Text = "BagMRU Size";
        // 
        // bagMruSizeNumeric
        // 
        bagMruSizeNumeric.Increment = new decimal(new int[] { 1000, 0, 0, 0 });
        bagMruSizeNumeric.Location = new Point(128, 27);
        bagMruSizeNumeric.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        bagMruSizeNumeric.Minimum = new decimal(new int[] { 5000, 0, 0, 0 });
        bagMruSizeNumeric.Name = "bagMruSizeNumeric";
        bagMruSizeNumeric.Size = new Size(120, 23);
        bagMruSizeNumeric.TabIndex = 1;
        bagMruSizeNumeric.Value = new decimal(new int[] { 50000, 0, 0, 0 });
        // 
        // backupRootLabel
        // 
        backupRootLabel.AutoSize = true;
        backupRootLabel.Location = new Point(14, 68);
        backupRootLabel.Name = "backupRootLabel";
        backupRootLabel.Size = new Size(104, 15);
        backupRootLabel.TabIndex = 2;
        backupRootLabel.Text = "バックアップ先";
        // 
        // backupRootTextBox
        // 
        backupRootTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        backupRootTextBox.Location = new Point(128, 64);
        backupRootTextBox.Name = "backupRootTextBox";
        backupRootTextBox.Size = new Size(709, 23);
        backupRootTextBox.TabIndex = 3;
        // 
        // browseBackupButton
        // 
        browseBackupButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        browseBackupButton.Location = new Point(849, 63);
        browseBackupButton.Name = "browseBackupButton";
        browseBackupButton.Size = new Size(92, 26);
        browseBackupButton.TabIndex = 4;
        browseBackupButton.Text = "選択...";
        browseBackupButton.UseVisualStyleBackColor = true;
        browseBackupButton.Click += BrowseBackupButton_Click;
        // 
        // restartExplorerCheckBox
        // 
        restartExplorerCheckBox.AutoSize = true;
        restartExplorerCheckBox.Checked = true;
        restartExplorerCheckBox.CheckState = CheckState.Checked;
        restartExplorerCheckBox.Location = new Point(128, 101);
        restartExplorerCheckBox.Name = "restartExplorerCheckBox";
        restartExplorerCheckBox.Size = new Size(194, 19);
        restartExplorerCheckBox.TabIndex = 5;
        restartExplorerCheckBox.Text = "処理後にExplorerを再起動する";
        restartExplorerCheckBox.UseVisualStyleBackColor = true;
        // 
        // openBackupAfterCompletionCheckBox
        // 
        openBackupAfterCompletionCheckBox.AutoSize = true;
        openBackupAfterCompletionCheckBox.Checked = true;
        openBackupAfterCompletionCheckBox.CheckState = CheckState.Checked;
        openBackupAfterCompletionCheckBox.Location = new Point(343, 101);
        openBackupAfterCompletionCheckBox.Name = "openBackupAfterCompletionCheckBox";
        openBackupAfterCompletionCheckBox.Size = new Size(182, 19);
        openBackupAfterCompletionCheckBox.TabIndex = 6;
        openBackupAfterCompletionCheckBox.Text = "完了後にバックアップ先を開く";
        openBackupAfterCompletionCheckBox.UseVisualStyleBackColor = true;
        // 
        // verboseLogCheckBox
        // 
        verboseLogCheckBox.AutoSize = true;
        verboseLogCheckBox.Location = new Point(546, 101);
        verboseLogCheckBox.Name = "verboseLogCheckBox";
        verboseLogCheckBox.Size = new Size(102, 19);
        verboseLogCheckBox.TabIndex = 7;
        verboseLogCheckBox.Text = "詳細ログを表示";
        verboseLogCheckBox.UseVisualStyleBackColor = true;
        // 
        // buttonPanel
        // 
        buttonPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        buttonPanel.Controls.Add(precheckButton);
        buttonPanel.Controls.Add(backupOnlyButton);
        buttonPanel.Controls.Add(repairButton);
        buttonPanel.Controls.Add(restartExplorerButton);
        buttonPanel.Controls.Add(openBackupButton);
        buttonPanel.Controls.Add(restoreButton);
        buttonPanel.Controls.Add(saveLogButton);
        buttonPanel.Controls.Add(exitButton);
        buttonPanel.Location = new Point(12, 224);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(960, 72);
        buttonPanel.TabIndex = 2;
        // 
        // precheckButton
        // 
        precheckButton.Location = new Point(3, 3);
        precheckButton.Name = "precheckButton";
        precheckButton.Size = new Size(112, 30);
        precheckButton.TabIndex = 0;
        precheckButton.Text = "事前チェック";
        precheckButton.UseVisualStyleBackColor = true;
        precheckButton.Click += PrecheckButton_Click;
        // 
        // backupOnlyButton
        // 
        backupOnlyButton.Location = new Point(121, 3);
        backupOnlyButton.Name = "backupOnlyButton";
        backupOnlyButton.Size = new Size(132, 30);
        backupOnlyButton.TabIndex = 1;
        backupOnlyButton.Text = "バックアップのみ実行";
        backupOnlyButton.UseVisualStyleBackColor = true;
        backupOnlyButton.Click += BackupOnlyButton_Click;
        // 
        // repairButton
        // 
        repairButton.Location = new Point(259, 3);
        repairButton.Name = "repairButton";
        repairButton.Size = new Size(112, 30);
        repairButton.TabIndex = 2;
        repairButton.Text = "修復を実行";
        repairButton.UseVisualStyleBackColor = true;
        repairButton.Click += RepairButton_Click;
        // 
        // restartExplorerButton
        // 
        restartExplorerButton.Location = new Point(377, 3);
        restartExplorerButton.Name = "restartExplorerButton";
        restartExplorerButton.Size = new Size(132, 30);
        restartExplorerButton.TabIndex = 3;
        restartExplorerButton.Text = "Explorerを再起動";
        restartExplorerButton.UseVisualStyleBackColor = true;
        restartExplorerButton.Click += RestartExplorerButton_Click;
        // 
        // openBackupButton
        // 
        openBackupButton.Location = new Point(515, 3);
        openBackupButton.Name = "openBackupButton";
        openBackupButton.Size = new Size(132, 30);
        openBackupButton.TabIndex = 4;
        openBackupButton.Text = "バックアップ先を開く";
        openBackupButton.UseVisualStyleBackColor = true;
        openBackupButton.Click += OpenBackupButton_Click;
        // 
        // restoreButton
        // 
        restoreButton.Location = new Point(653, 3);
        restoreButton.Name = "restoreButton";
        restoreButton.Size = new Size(112, 30);
        restoreButton.TabIndex = 5;
        restoreButton.Text = "復元";
        restoreButton.UseVisualStyleBackColor = true;
        restoreButton.Click += RestoreButton_Click;
        // 
        // saveLogButton
        // 
        saveLogButton.Location = new Point(771, 3);
        saveLogButton.Name = "saveLogButton";
        saveLogButton.Size = new Size(100, 30);
        saveLogButton.TabIndex = 6;
        saveLogButton.Text = "ログを保存";
        saveLogButton.UseVisualStyleBackColor = true;
        saveLogButton.Click += SaveLogButton_Click;
        // 
        // exitButton
        // 
        exitButton.Location = new Point(877, 3);
        exitButton.Name = "exitButton";
        exitButton.Size = new Size(72, 30);
        exitButton.TabIndex = 7;
        exitButton.Text = "終了";
        exitButton.UseVisualStyleBackColor = true;
        exitButton.Click += ExitButton_Click;
        // 
        // statusLabel
        // 
        statusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        statusLabel.Location = new Point(12, 304);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(960, 20);
        statusLabel.TabIndex = 3;
        statusLabel.Text = "待機中";
        // 
        // logTextBox
        // 
        logTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        logTextBox.ContextMenuStrip = logContextMenu;
        logTextBox.Font = new Font("Consolas", 9F);
        logTextBox.Location = new Point(12, 327);
        logTextBox.Multiline = true;
        logTextBox.Name = "logTextBox";
        logTextBox.ReadOnly = true;
        logTextBox.ScrollBars = ScrollBars.Vertical;
        logTextBox.Size = new Size(960, 322);
        logTextBox.TabIndex = 4;
        // 
        // logContextMenu
        // 
        logContextMenu.Items.AddRange(new ToolStripItem[] { selectAllMenuItem, copyMenuItem, clearMenuItem });
        logContextMenu.Name = "logContextMenu";
        logContextMenu.Size = new Size(123, 70);
        // 
        // selectAllMenuItem
        // 
        selectAllMenuItem.Name = "selectAllMenuItem";
        selectAllMenuItem.Size = new Size(122, 22);
        selectAllMenuItem.Text = "全選択";
        selectAllMenuItem.Click += SelectAllMenuItem_Click;
        // 
        // copyMenuItem
        // 
        copyMenuItem.Name = "copyMenuItem";
        copyMenuItem.Size = new Size(122, 22);
        copyMenuItem.Text = "コピー";
        copyMenuItem.Click += CopyMenuItem_Click;
        // 
        // clearMenuItem
        // 
        clearMenuItem.Name = "clearMenuItem";
        clearMenuItem.Size = new Size(122, 22);
        clearMenuItem.Text = "クリア";
        clearMenuItem.Click += ClearMenuItem_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(984, 661);
        Controls.Add(logTextBox);
        Controls.Add(statusLabel);
        Controls.Add(buttonPanel);
        Controls.Add(settingsGroup);
        Controls.Add(descriptionLabel);
        MinimumSize = new Size(900, 620);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Explorer フォルダ表示記憶リセットツール v1.0.0";
        FormClosing += MainForm_FormClosing;
        settingsGroup.ResumeLayout(false);
        settingsGroup.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)bagMruSizeNumeric).EndInit();
        buttonPanel.ResumeLayout(false);
        logContextMenu.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
}
