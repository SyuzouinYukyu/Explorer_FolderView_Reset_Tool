using System.Diagnostics;
using Microsoft.Win32;

namespace Explorer_FolderView_Reset_Tool;

public sealed class RegistryBackupService
{
    private readonly LogService _log;

    public RegistryBackupService(LogService log)
    {
        _log = log;
    }

    public async Task<BackupResult> BackupAsync(string backupRoot, CancellationToken cancellationToken)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupDir = Path.Combine(backupRoot, timestamp);
        Directory.CreateDirectory(backupDir);

        _log.Info($"バックアップ先: {backupDir}");
        var failures = new List<string>();
        var exportedFiles = new List<string>();

        foreach (var key in ExplorerFolderViewService.FolderViewKeys)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _log.Info($"バックアップ確認: {key}");

            if (!RegistryKeyExists(key))
            {
                _log.Info($"キーなし: {key}");
                continue;
            }

            var fileName = MakeSafeFileName(key) + ".reg";
            var destination = Path.Combine(backupDir, fileName);
            var result = await RunRegExeAsync(
                ["export", key, destination, "/y"],
                cancellationToken).ConfigureAwait(false);

            if (result.ExitCode == 0 && File.Exists(destination))
            {
                _log.Success($"バックアップ済: {destination}");
                exportedFiles.Add(destination);
            }
            else
            {
                var message = $"バックアップ失敗: {key} / ExitCode={result.ExitCode} / {result.ErrorText} {result.OutputText}".Trim();
                _log.Error(message);
                failures.Add(message);
            }
        }

        return new BackupResult(backupDir, exportedFiles, failures);
    }

    public async Task<ImportResult> ImportBackupFolderAsync(string backupDir, CancellationToken cancellationToken)
    {
        var files = Directory.Exists(backupDir)
            ? Directory.GetFiles(backupDir, "*.reg", SearchOption.TopDirectoryOnly).OrderBy(x => x).ToArray()
            : [];

        if (files.Length == 0)
        {
            _log.Warning($"復元対象の .reg が見つかりません: {backupDir}");
            return new ImportResult(0, 0);
        }

        var succeeded = 0;
        var failed = 0;

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _log.Info($"インポート: {file}");

            var result = await RunRegExeAsync(["import", file], cancellationToken).ConfigureAwait(false);
            if (result.ExitCode == 0)
            {
                succeeded++;
                _log.Success($"インポート成功: {Path.GetFileName(file)}");
            }
            else
            {
                failed++;
                _log.Error($"インポート失敗: {Path.GetFileName(file)} / ExitCode={result.ExitCode} / {result.ErrorText} {result.OutputText}".Trim());
            }
        }

        return new ImportResult(succeeded, failed);
    }

    public async Task<RegCommandResult> AddDwordValueAsync(string keyPath, string valueName, int value, CancellationToken cancellationToken)
    {
        return await RunRegExeAsync(
            ["add", keyPath, "/v", valueName, "/t", "REG_DWORD", "/d", value.ToString(), "/f"],
            cancellationToken).ConfigureAwait(false);
    }

    private static bool RegistryKeyExists(string keyPath)
    {
        using var key = OpenCurrentUserSubKey(keyPath, writable: false);
        return key is not null;
    }

    internal static RegistryKey? OpenCurrentUserSubKey(string keyPath, bool writable)
    {
        const string prefix = @"HKCU\";
        if (!keyPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("HKCU 以外のキーは操作しません。");
        }

        return Registry.CurrentUser.OpenSubKey(keyPath[prefix.Length..], writable);
    }

    internal static string ToCurrentUserSubKeyPath(string keyPath)
    {
        const string prefix = @"HKCU\";
        if (!keyPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("HKCU 以外のキーは操作しません。");
        }

        return keyPath[prefix.Length..];
    }

    private static string MakeSafeFileName(string key)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var chars = key.Select(ch => invalid.Contains(ch) || ch is '\\' or '/' or ':' or '*' or '?' or '"' or '<' or '>' or '|' or ' '
            ? '_'
            : ch).ToArray();
        return new string(chars);
    }

    private static async Task<RegCommandResult> RunRegExeAsync(string[] arguments, CancellationToken cancellationToken)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "reg.exe",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        foreach (var argument in arguments)
        {
            process.StartInfo.ArgumentList.Add(argument);
        }

        process.Start();
        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        var output = await outputTask.ConfigureAwait(false);
        var error = await errorTask.ConfigureAwait(false);
        return new RegCommandResult(process.ExitCode, output.Trim(), error.Trim());
    }
}

public sealed record BackupResult(string BackupDirectory, IReadOnlyList<string> ExportedFiles, IReadOnlyList<string> Failures)
{
    public bool HasFailures => Failures.Count > 0;
}

public sealed record ImportResult(int Succeeded, int Failed);

public sealed record RegCommandResult(int ExitCode, string OutputText, string ErrorText);
