namespace Explorer_FolderView_Reset_Tool;

public sealed class AppSettings
{
    public int BagMruSize { get; set; } = 50000;

    public string BackupRoot { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
        "Explorer_FolderView_Backup");

    public bool RestartExplorerAfterRepair { get; set; } = true;

    public bool OpenBackupAfterCompletion { get; set; } = true;

    public bool ShowVerboseLog { get; set; }

    public int? WindowLeft { get; set; }

    public int? WindowTop { get; set; }

    public int? WindowWidth { get; set; }

    public int? WindowHeight { get; set; }
}
