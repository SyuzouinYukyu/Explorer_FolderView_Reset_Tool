# AGENTS.md

## Project

Explorer_FolderView_Reset_Tool_v1.0.0

This project is a Windows 11 utility for repairing Explorer folder view memory by backing up and resetting the current user's Shell Bags / BagMRU registry data.

## Environment

- Target OS: Windows 11 x64
- UI language: Japanese
- Primary implementation language: C#
- UI framework: Windows Forms
- Target framework: .NET 8
- Build configuration: Release
- Publish target: win-x64
- Publish mode: self-contained
- Output format: single-file EXE

## Core Rules

- Treat this as a Windows 11 x64-only tool.
- Keep all UI labels and user-facing messages in Japanese.
- Preserve existing features when modifying the project.
- Prefer complete, buildable, production-oriented changes.
- Do not leave partial implementations, TODO-only stubs, or pseudocode.
- Do not add unrelated features.
- Do not rewrite the project unnecessarily.
- Do not change project structure unless there is a clear technical reason.

## Registry Safety Rules

- Registry operations must be limited to HKCU only.
- HKLM operations are strictly prohibited.
- Do not add, modify, or delete arbitrary registry keys based on user input.
- Registry deletion targets must be hardcoded and limited to the intended Explorer folder view keys.
- The intended deletion targets are only:

    HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags
    HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU
    HKCU\Software\Microsoft\Windows\Shell\Bags
    HKCU\Software\Microsoft\Windows\Shell\BagMRU
    HKCU\Software\Microsoft\Windows\ShellNoRoam\Bags
    HKCU\Software\Microsoft\Windows\ShellNoRoam\BagMRU

- The intended BagMRU Size setting targets are only:

    HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell
    HKCU\Software\Microsoft\Windows\Shell

- The default BagMRU Size value is 50000.
- Allow the user to change BagMRU Size only within a safe GUI-defined numeric range.
- Do not require administrator privileges.
- Do not request UAC elevation by default.
- If a registry operation fails, log the exact target key and exception message.
- Main repair must fail clearly if the primary key cannot be configured.
- Compatible/legacy key failures may be treated as warnings if the primary key succeeds.

## File Safety Rules

- Do not delete user files.
- Do not modify files outside the project directory except for user-selected backup/log output folders.
- Creating backup folders and log files is allowed.
- Do not overwrite existing backups.
- Backup folders must use timestamped subfolders.
- Do not hardcode personal credentials, tokens, passwords, API keys, or private paths.
- Do not add external communication, telemetry, update checks, or analytics.

## Explorer Process Rules

- If Explorer.exe is stopped, the code must always attempt to restart Explorer.exe in a finally-equivalent path.
- Explorer restart failures must be logged clearly.
- Provide a separate “Explorerを再起動” operation.
- Do not kill unrelated processes.
- Do not reboot Windows automatically.
- Windows restart is usually not required.
- Recommend sign-out or restart only if Explorer still behaves incorrectly after repair.

## Implementation Rules

- Do not implement this tool as a PowerShell wrapper.
- Implement registry operations directly in C# using Microsoft.Win32.Registry APIs where appropriate.
- reg.exe may be used only for:
  - exporting .reg backups
  - importing .reg backups
  - fallback registry add operations when Registry API fails for compatible HKCU keys
- Do not use external NuGet packages unless explicitly approved.
- Use only standard .NET libraries unless instructed otherwise.
- Use async/await or background tasks for long-running operations so the UI does not freeze.
- Exceptions must be caught and logged.
- The application must not crash on normal operational errors.
- Do not perform destructive actions without a confirmation dialog.
- Do not perform repair automatically at startup.
- Do not perform restore automatically at startup.

## Logging Rules

- Show logs in the GUI.
- Save logs as UTF-8 with BOM for Japanese Windows compatibility.
- Log at least:
  - start time
  - end time
  - PC name
  - Windows version
  - user name
  - backup directory
  - target registry keys
  - success/failure per operation
  - exception messages
- Logs must be human-readable.
- Include a way to copy or save the displayed log.
- Do not log secrets, tokens, passwords, or personal credentials.

## UI Rules

- Use Windows Forms.
- Use Japanese labels.
- Provide clear warning/confirmation dialogs before repair or restore operations.
- Main form title should include the version.
- Avoid cramped layouts.
- Use readable fonts and enough spacing.
- Include:
  - explanation area
  - BagMRU Size setting
  - backup folder selector
  - pre-check button
  - backup-only button
  - repair execution button
  - restore-from-backup button
  - Explorer restart button
  - open backup folder button
  - log area
  - exit button
- The log area should support copy operations.
- The UI must remain responsive during backup, repair, restore, and publish-related operations.

## Restore Rules

- Restore must require explicit user confirmation.
- Restore should import .reg files from a selected backup folder.
- Restore should use reg.exe import.
- Restore operation must not silently proceed if no .reg files are found.
- After restore, recommend Explorer restart.
- Do not auto-reboot Windows.
- Do not import registry files from arbitrary locations without explicit user selection.

## C# Project Rules

- TargetFramework must be net8.0-windows.
- UseWindowsForms must be true.
- Nullable should be enabled.
- ImplicitUsings should be enabled.
- ApplicationConfiguration.Initialize() should be used.
- Build must succeed in Release configuration.
- Publish must support:
  - win-x64
  - self-contained
  - single-file EXE
- Keep Designer.cs and MainForm.cs consistent.
- Do not manually corrupt Designer-generated layout code.
- Do not add unknown resources or icons unless explicitly requested.
- Application name should remain Explorer_FolderView_Reset_Tool_v1.0.0 unless explicitly instructed.

## Build Helper Script Rules

- If build_publish_win-x64.ps1 exists, do not delete it.
- If build or publish conditions change, update build_publish_win-x64.ps1 accordingly.
- build_publish_win-x64.ps1 should be able to run:
  - dotnet restore
  - dotnet build -c Release
  - dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
- After publish, report the generated EXE path.
- If build or publish fails, report the exact error and the failed command.

## Git Rules

- Do not run git push.
- Do not create remote repositories.
- Do not publish to GitHub unless explicitly instructed.
- Do not create tags, releases, or external publications unless explicitly instructed.
- Creating or updating .gitignore, README.md, and CHANGELOG.md is allowed.
- Commit only when the user explicitly instructs it.
- Do not include generated backups, logs, bin folders, or obj folders in commits unless explicitly instructed.

## Documentation Rules

Maintain or create the following files as needed:

- README.md
- CHANGELOG.md
- AGENTS.md

README.md should include:

- tool purpose
- target OS
- what the tool repairs
- registry keys touched by the tool
- what the tool does not delete
- backup behavior
- restore behavior
- how to run
- how to verify repair success
- that Windows restart is usually not required
- that sign-out or restart can be used if Explorer still behaves incorrectly
- known interference factors such as Windhawk, ExplorerPatcher, StartAllBack, QTTabBar, and shell extensions

CHANGELOG.md should record versioned changes.

## Completion Criteria

A task is complete only when all applicable conditions are met:

- The requested implementation is complete.
- Release build succeeds.
- win-x64 / self-contained / single-file EXE publish succeeds when applicable.
- No prohibited registry scope is used.
- HKLM operations are not introduced.
- User file deletion logic is not introduced.
- Unapproved external NuGet packages are not added.
- The tool is not implemented as a PowerShell wrapper.
- Created and changed files are listed.
- Build result is reported.
- Publish result is reported.
- Run instructions are reported.
- Verification steps are reported.
- Remaining risks are reported.

## Reporting Format

After completing changes, report in this format:

    作成/変更ファイル
    ビルド結果
    発行結果
    実行方法
    確認方法
    残リスク