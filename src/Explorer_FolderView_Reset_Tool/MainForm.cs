using System.Diagnostics;

namespace Explorer_FolderView_Reset_Tool;

public partial class MainForm : Form
{
    private readonly SettingsService _settingsService = new();
    private readonly LogService _log = new();
    private readonly RegistryBackupService _backupService;
    private readonly ExplorerFolderViewService _explorerService;
    private AppSettings _settings;
    private string? _lastBackupDirectory;
    private bool _operationRunning;

    public MainForm()
    {
        InitializeComponent();
        ApplyEmbeddedApplicationIcon();

        _settings = _settingsService.Load();
        _backupService = new RegistryBackupService(_log);
        _explorerService = new ExplorerFolderViewService(_log, _backupService);
        _log.LineAdded += Log_LineAdded;

        LoadSettingsToUi();
        _log.Info("アプリを起動しました。");
        _log.Info("操作対象は現在ユーザー HKCU の Explorer 表示記憶のみです。HKLM とユーザーファイルは操作しません。");
    }

    private void ApplyEmbeddedApplicationIcon()
    {
        try
        {
            var icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (icon is not null)
            {
                Icon = icon;
            }
        }
        catch
        {
            // アイコン設定に失敗しても修復ツール本体の動作を止めない。
        }
    }

    private void LoadSettingsToUi()
    {
        bagMruSizeNumeric.Value = Math.Clamp(_settings.BagMruSize, 5000, 100000);
        backupRootTextBox.Text = string.IsNullOrWhiteSpace(_settings.BackupRoot)
            ? new AppSettings().BackupRoot
            : _settings.BackupRoot;
        restartExplorerCheckBox.Checked = _settings.RestartExplorerAfterRepair;
        openBackupAfterCompletionCheckBox.Checked = _settings.OpenBackupAfterCompletion;
        verboseLogCheckBox.Checked = _settings.ShowVerboseLog;
        disableFolderTypeAutoDetectionCheckBox.Checked = _settings.DisableFolderTypeAutoDetection;

        if (_settings.WindowWidth is > 0 && _settings.WindowHeight is > 0)
        {
            StartPosition = FormStartPosition.Manual;
            Bounds = new Rectangle(
                _settings.WindowLeft ?? Left,
                _settings.WindowTop ?? Top,
                Math.Max(_settings.WindowWidth.Value, MinimumSize.Width),
                Math.Max(_settings.WindowHeight.Value, MinimumSize.Height));
        }
    }

    private void SaveSettingsFromUi()
    {
        _settings.BagMruSize = (int)bagMruSizeNumeric.Value;
        _settings.BackupRoot = backupRootTextBox.Text.Trim();
        _settings.RestartExplorerAfterRepair = restartExplorerCheckBox.Checked;
        _settings.OpenBackupAfterCompletion = openBackupAfterCompletionCheckBox.Checked;
        _settings.ShowVerboseLog = verboseLogCheckBox.Checked;
        _settings.DisableFolderTypeAutoDetection = disableFolderTypeAutoDetectionCheckBox.Checked;

        if (WindowState == FormWindowState.Normal)
        {
            _settings.WindowLeft = Left;
            _settings.WindowTop = Top;
            _settings.WindowWidth = Width;
            _settings.WindowHeight = Height;
        }

        try
        {
            _settingsService.Save(_settings);
        }
        catch (Exception ex)
        {
            _log.Exception(ex, "設定JSONの保存に失敗しました");
        }
    }

    private void Log_LineAdded(string line, LogLevel level)
    {
        if (level == LogLevel.Verbose && !verboseLogCheckBox.Checked)
        {
            return;
        }

        if (IsDisposed)
        {
            return;
        }

        if (InvokeRequired && IsHandleCreated)
        {
            BeginInvoke((MethodInvoker)(() => AppendLogLine(line)));
            return;
        }

        AppendLogLine(line);
    }

    private void AppendLogLine(string line)
    {
        logTextBox.AppendText(line + Environment.NewLine);
    }

    private async void PrecheckButton_Click(object? sender, EventArgs e)
    {
        await RunOperationAsync("事前チェック中...", async token =>
        {
            SaveSettingsFromUi();
            await _explorerService.RunPrecheckAsync(backupRootTextBox.Text.Trim(), token);
        });
    }

    private async void BackupOnlyButton_Click(object? sender, EventArgs e)
    {
        await RunOperationAsync("バックアップ中...", async token =>
        {
            SaveSettingsFromUi();
            var result = await _backupService.BackupAsync(backupRootTextBox.Text.Trim(), token);
            _lastBackupDirectory = result.BackupDirectory;
            SaveLogToBackupDirectory(result.BackupDirectory);

            if (result.HasFailures)
            {
                _log.Warning("バックアップのみ実行は完了しましたが、失敗したキーがあります。");
            }
            else
            {
                _log.Success("バックアップのみ実行が完了しました。");
            }

            if (openBackupAfterCompletionCheckBox.Checked)
            {
                OpenFolder(result.BackupDirectory);
            }
        });
    }

    private async void RepairButton_Click(object? sender, EventArgs e)
    {
        var message = "Explorer の Bags / BagMRU レジストリキーをバックアップ後に削除し、BagMRU Size を設定します。" +
            Environment.NewLine + Environment.NewLine +
            "ファイルやフォルダ本体は削除しませんが、フォルダごとの表示記憶はリセットされます。実行しますか？";

        if (MessageBox.Show(this, message, "修復実行の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }

        await RunOperationAsync("修復中...", async token =>
        {
            SaveSettingsFromUi();
            var result = await _explorerService.RepairAsync(
                backupRootTextBox.Text.Trim(),
                (int)bagMruSizeNumeric.Value,
                restartExplorerCheckBox.Checked,
                disableFolderTypeAutoDetectionCheckBox.Checked,
                ConfirmContinueAfterBackupFailureAsync,
                token);

            _lastBackupDirectory = result.BackupDirectory;
            SaveLogToBackupDirectory(result.BackupDirectory);

            if (openBackupAfterCompletionCheckBox.Checked)
            {
                OpenFolder(result.BackupDirectory);
            }

            var completion = result.ModernBagMruSizeSet
                ? "修復処理が完了しました。"
                : "修復処理は完了しましたが、主要キーの BagMRU Size 設定に失敗しています。ログを確認してください。";

            MessageBox.Show(
                this,
                completion + Environment.NewLine + Environment.NewLine +
                "確認方法:" + Environment.NewLine +
                "1. 任意のフォルダを開く" + Environment.NewLine +
                "2. 表示形式を「詳細」「一覧」「中アイコン」などに変更する" + Environment.NewLine +
                "3. Explorerを閉じて同じフォルダを再度開く" + Environment.NewLine +
                "4. 表示形式が保存されているか確認する",
                "処理完了",
                MessageBoxButtons.OK,
                result.ModernBagMruSizeSet ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        });
    }

    private async void RestartExplorerButton_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show(this, "Explorer.exe を再起動します。開いているExplorerウィンドウは閉じられます。実行しますか？", "Explorer再起動の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }

        await RunOperationAsync("Explorer再起動中...", async token =>
        {
            await _explorerService.RestartExplorerAsync(token);
        });
    }

    private void OpenBackupButton_Click(object? sender, EventArgs e)
    {
        var path = _lastBackupDirectory;
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            path = backupRootTextBox.Text.Trim();
        }

        OpenFolder(path);
    }

    private async void RestoreButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "復元する .reg バックアップフォルダを選択してください。",
            UseDescriptionForTitle = true,
            SelectedPath = Directory.Exists(_lastBackupDirectory) ? _lastBackupDirectory : backupRootTextBox.Text.Trim()
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var warning = "選択した .reg ファイルを現在ユーザー HKCU へインポートします。" +
            Environment.NewLine + "現在の Explorer 表示記憶がバックアップ時点の状態に戻る可能性があります。" +
            Environment.NewLine + Environment.NewLine +
            "本当に復元しますか？";

        if (MessageBox.Show(this, warning, "復元の強い確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }

        await RunOperationAsync("復元中...", async token =>
        {
            var result = await _backupService.ImportBackupFolderAsync(dialog.SelectedPath, token);
            _log.Info($"復元結果: 成功 {result.Succeeded} / 失敗 {result.Failed}");
            MessageBox.Show(this, "復元処理が完了しました。Explorerの再起動、またはサインアウト/再起動後に状態を確認してください。", "復元完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        });
    }

    private void SaveLogButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Title = "ログを保存",
            Filter = "ログファイル (*.log)|*.log|テキストファイル (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
            FileName = $"Explorer_FolderView_Reset_Tool_{DateTime.Now:yyyyMMdd_HHmmss}.log",
            InitialDirectory = Directory.Exists(_lastBackupDirectory) ? _lastBackupDirectory : backupRootTextBox.Text.Trim()
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            _log.SaveToFile(dialog.FileName);
            _log.Success($"ログを保存しました: {dialog.FileName}");
        }
        catch (Exception ex)
        {
            _log.Exception(ex, "ログ保存に失敗しました");
            MessageBox.Show(this, ex.Message, "ログ保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BrowseBackupButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "バックアップ先フォルダを選択してください。",
            UseDescriptionForTitle = true,
            SelectedPath = Directory.Exists(backupRootTextBox.Text.Trim())
                ? backupRootTextBox.Text.Trim()
                : Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            backupRootTextBox.Text = dialog.SelectedPath;
            SaveSettingsFromUi();
        }
    }

    private void ExitButton_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_operationRunning)
        {
            MessageBox.Show(this, "処理中は終了できません。完了まで待ってください。", "処理中", MessageBoxButtons.OK, MessageBoxIcon.Information);
            e.Cancel = true;
            return;
        }

        SaveSettingsFromUi();
    }

    private async Task RunOperationAsync(string status, Func<CancellationToken, Task> operation)
    {
        if (_operationRunning)
        {
            return;
        }

        _operationRunning = true;
        SetButtonsEnabled(false);
        statusLabel.Text = status;

        try
        {
            await operation(CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            _log.Warning("処理がキャンセルされました。");
        }
        catch (Exception ex)
        {
            _log.Exception(ex, "処理中に予期しない例外が発生しました");
            MessageBox.Show(this, ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            statusLabel.Text = "待機中";
            SetButtonsEnabled(true);
            _operationRunning = false;
        }
    }

    private Task<bool> ConfirmContinueAfterBackupFailureAsync(BackupResult result)
    {
        if (InvokeRequired)
        {
            var tcs = new TaskCompletionSource<bool>();
            BeginInvoke((MethodInvoker)(() =>
            {
                try
                {
                    tcs.SetResult(ShowBackupFailureDialog(result));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        return Task.FromResult(ShowBackupFailureDialog(result));
    }

    private bool ShowBackupFailureDialog(BackupResult result)
    {
        var text = "バックアップに失敗した対象があります。" + Environment.NewLine +
            "通常は修復を中止することを推奨します。" + Environment.NewLine + Environment.NewLine +
            "明示的に続行しますか？";
        return MessageBox.Show(this, text, "バックアップ失敗", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
    }

    private void SaveLogToBackupDirectory(string backupDirectory)
    {
        try
        {
            var logPath = Path.Combine(backupDirectory, "Explorer_FolderView_Reset_Tool.log");
            _log.SaveToFile(logPath);
            _log.Success($"ログファイル保存: {logPath}");
        }
        catch (Exception ex)
        {
            _log.Exception(ex, "バックアップ先へのログ保存に失敗しました");
        }
    }

    private void OpenFolder(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new DirectoryNotFoundException("バックアップ先が空です。");
            }

            Directory.CreateDirectory(path);
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            _log.Exception(ex, "バックアップ先を開けませんでした");
            MessageBox.Show(this, ex.Message, "フォルダを開けません", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetButtonsEnabled(bool enabled)
    {
        foreach (Control control in buttonPanel.Controls)
        {
            control.Enabled = enabled;
        }

        browseBackupButton.Enabled = enabled;
        bagMruSizeNumeric.Enabled = enabled;
        backupRootTextBox.Enabled = enabled;
        restartExplorerCheckBox.Enabled = enabled;
        openBackupAfterCompletionCheckBox.Enabled = enabled;
        verboseLogCheckBox.Enabled = enabled;
        disableFolderTypeAutoDetectionCheckBox.Enabled = enabled;
    }

    private void SelectAllMenuItem_Click(object? sender, EventArgs e)
    {
        logTextBox.SelectAll();
    }

    private void CopyMenuItem_Click(object? sender, EventArgs e)
    {
        logTextBox.Copy();
    }

    private void ClearMenuItem_Click(object? sender, EventArgs e)
    {
        _log.Clear();
        logTextBox.Clear();
    }
}
