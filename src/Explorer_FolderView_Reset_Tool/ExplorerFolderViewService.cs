using System.Diagnostics;
using Microsoft.Win32;

namespace Explorer_FolderView_Reset_Tool;

public sealed class ExplorerFolderViewService
{
    public static readonly string[] FolderViewKeys =
    [
        @"HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags",
        @"HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU",
        @"HKCU\Software\Microsoft\Windows\Shell\Bags",
        @"HKCU\Software\Microsoft\Windows\Shell\BagMRU",
        @"HKCU\Software\Microsoft\Windows\ShellNoRoam\Bags",
        @"HKCU\Software\Microsoft\Windows\ShellNoRoam\BagMRU"
    ];

    public const string ModernShellKey = @"HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell";
    public const string ClassicShellKey = @"HKCU\Software\Microsoft\Windows\Shell";
    public const string BagMruSizeValueName = "BagMRU Size";

    private readonly LogService _log;
    private readonly RegistryBackupService _backupService;

    public ExplorerFolderViewService(LogService log, RegistryBackupService backupService)
    {
        _log = log;
        _backupService = backupService;
    }

    public Task RunPrecheckAsync(string backupRoot, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            _log.Info("事前チェックを開始します。");
            _log.Info($"開始時刻: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _log.Info($"PC名: {Environment.MachineName}");
            _log.Info($"ユーザー名: {Environment.UserDomainName}\\{Environment.UserName}");
            _log.Info($"OS: {Environment.OSVersion.VersionString}");
            _log.Info($"64 bit OS: {Environment.Is64BitOperatingSystem}");

            foreach (var keyPath in FolderViewKeys)
            {
                using var key = RegistryBackupService.OpenCurrentUserSubKey(keyPath, writable: false);
                _log.Info($"{keyPath}: {(key is null ? "キーなし" : "存在")}");
            }

            _log.Info($"{ModernShellKey} / {BagMruSizeValueName}: {ReadBagMruSize(ModernShellKey)}");
            _log.Info($"{ClassicShellKey} / {BagMruSizeValueName}: {ReadBagMruSize(ClassicShellKey)}");
            _log.Info($"Explorer.exe 起動状態: {(Process.GetProcessesByName("explorer").Length > 0 ? "起動中" : "未起動")}");

            CheckBackupWritable(backupRoot);
            _log.Success("事前チェックが完了しました。");
        }, cancellationToken);
    }

    public async Task<RepairResult> RepairAsync(
        string backupRoot,
        int bagMruSize,
        bool restartExplorer,
        Func<BackupResult, Task<bool>> shouldContinueAfterBackupFailureAsync,
        CancellationToken cancellationToken)
    {
        _log.Info("修復処理を開始します。");
        _log.Info($"開始時刻: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        _log.Info($"PC名: {Environment.MachineName}");
        _log.Info($"ユーザー名: {Environment.UserDomainName}\\{Environment.UserName}");
        _log.Info($"BagMRU Size 設定値: {bagMruSize}");

        var backupResult = await _backupService.BackupAsync(backupRoot, cancellationToken).ConfigureAwait(false);
        if (backupResult.HasFailures)
        {
            _log.Warning("バックアップ失敗があるため、修復を中断するか確認します。");
            var shouldContinue = await shouldContinueAfterBackupFailureAsync(backupResult).ConfigureAwait(false);
            if (!shouldContinue)
            {
                _log.Warning("ユーザー操作により修復を中止しました。");
                return new RepairResult(backupResult.BackupDirectory, false, false, false);
            }
        }

        var explorerWasStopped = false;
        var modernResult = false;
        var classicResult = false;

        try
        {
            StopExplorer();
            explorerWasStopped = true;
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken).ConfigureAwait(false);

            DeleteFolderViewKeys();
            modernResult = await SetBagMruSizeAsync(ModernShellKey, bagMruSize, critical: true, cancellationToken).ConfigureAwait(false);
            classicResult = await SetBagMruSizeAsync(ClassicShellKey, bagMruSize, critical: false, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _log.Exception(ex, "修復処理中に例外が発生しました");
        }
        finally
        {
            if (restartExplorer || explorerWasStopped)
            {
                TryStartExplorer();
            }

            _log.Info($"終了時刻: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        if (modernResult)
        {
            _log.Success("主要キーの BagMRU Size 設定: 成功");
        }
        else
        {
            _log.Error("主要キーの BagMRU Size 設定: 失敗");
        }

        if (classicResult)
        {
            _log.Success("互換キーの BagMRU Size 設定: 成功");
        }
        else
        {
            _log.Warning("互換キーの BagMRU Size 設定: スキップまたは失敗");
        }

        return new RepairResult(backupResult.BackupDirectory, true, modernResult, classicResult);
    }

    public Task RestartExplorerAsync(CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            _log.Info("Explorer を再起動します。");
            StopExplorer();
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken).ConfigureAwait(false);
            TryStartExplorer();
        }, cancellationToken);
    }

    private static string ReadBagMruSize(string keyPath)
    {
        try
        {
            using var key = RegistryBackupService.OpenCurrentUserSubKey(keyPath, writable: false);
            var value = key?.GetValue(BagMruSizeValueName);
            return value is null ? "未設定" : value.ToString() ?? "未設定";
        }
        catch (Exception ex)
        {
            return $"取得失敗: {ex.Message}";
        }
    }

    private void CheckBackupWritable(string backupRoot)
    {
        Directory.CreateDirectory(backupRoot);
        _log.Success($"バックアップ先フォルダ作成可: {backupRoot}");
    }

    private void StopExplorer()
    {
        var explorers = Process.GetProcessesByName("explorer");
        if (explorers.Length == 0)
        {
            _log.Warning("Explorer.exe は起動していません。");
            return;
        }

        foreach (var process in explorers)
        {
            try
            {
                _log.Info($"Explorer 停止: PID {process.Id}");
                process.Kill(entireProcessTree: false);
            }
            catch (Exception ex)
            {
                _log.Exception(ex, $"Explorer 停止失敗: PID {process.Id}");
            }
            finally
            {
                process.Dispose();
            }
        }
    }

    private void TryStartExplorer()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                UseShellExecute = true
            });
            _log.Success("Explorer を起動しました。");
        }
        catch (Exception ex)
        {
            _log.Exception(ex, "Explorer の起動に失敗しました。手動で explorer.exe を起動してください");
        }
    }

    private void DeleteFolderViewKeys()
    {
        foreach (var keyPath in FolderViewKeys)
        {
            try
            {
                var subKeyPath = RegistryBackupService.ToCurrentUserSubKeyPath(keyPath);
                using var existing = Registry.CurrentUser.OpenSubKey(subKeyPath, writable: false);
                if (existing is null)
                {
                    _log.Info($"削除対象なし: {keyPath}");
                    continue;
                }

                existing.Dispose();
                Registry.CurrentUser.DeleteSubKeyTree(subKeyPath, throwOnMissingSubKey: false);
                _log.Success($"削除成功: {keyPath}");
            }
            catch (Exception ex)
            {
                _log.Exception(ex, $"削除失敗: {keyPath}");
            }
        }
    }

    private async Task<bool> SetBagMruSizeAsync(string keyPath, int bagMruSize, bool critical, CancellationToken cancellationToken)
    {
        try
        {
            var subKeyPath = RegistryBackupService.ToCurrentUserSubKeyPath(keyPath);
            using var key = Registry.CurrentUser.CreateSubKey(subKeyPath, writable: true);
            if (key is null)
            {
                throw new InvalidOperationException("Registry.CurrentUser.CreateSubKey が null を返しました。");
            }

            key.SetValue(BagMruSizeValueName, bagMruSize, RegistryValueKind.DWord);
            _log.Success($"{(critical ? "主要キー" : "互換キー")} の BagMRU Size を Registry API で設定しました: {keyPath}");
            return true;
        }
        catch (UnauthorizedAccessException ex) when (!critical)
        {
            _log.Warning($"互換キーの Registry API 設定に失敗しました。reg.exe add で再試行します: {ex.Message}");
            return await SetBagMruSizeWithRegExeAsync(keyPath, bagMruSize, critical, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (!critical)
        {
            _log.Warning($"互換キーの Registry API 設定に失敗しました。reg.exe add で再試行します: {ex.Message}");
            return await SetBagMruSizeWithRegExeAsync(keyPath, bagMruSize, critical, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _log.Exception(ex, $"主要キーの BagMRU Size 設定に失敗しました: {keyPath}");
            return false;
        }
    }

    private async Task<bool> SetBagMruSizeWithRegExeAsync(string keyPath, int bagMruSize, bool critical, CancellationToken cancellationToken)
    {
        var result = await _backupService.AddDwordValueAsync(keyPath, BagMruSizeValueName, bagMruSize, cancellationToken).ConfigureAwait(false);
        if (result.ExitCode == 0)
        {
            _log.Success($"{(critical ? "主要キー" : "互換キー")} の BagMRU Size を reg.exe で設定しました: {keyPath}");
            return true;
        }

        var text = $"{result.ErrorText} {result.OutputText}".Trim();
        if (critical)
        {
            _log.Error($"主要キーの BagMRU Size 設定に失敗しました: ExitCode={result.ExitCode} {text}");
        }
        else
        {
            _log.Warning($"互換キーの BagMRU Size 設定は失敗しました: ExitCode={result.ExitCode} {text}");
        }

        return false;
    }
}

public sealed record RepairResult(string BackupDirectory, bool RepairExecuted, bool ModernBagMruSizeSet, bool ClassicBagMruSizeSet);
