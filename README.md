# Explorer_FolderView_Reset_Tool_v1.1.0

Windows 11 の Explorer がフォルダごとの表示形式を記憶しない場合に、現在ユーザーの Explorer 表示記憶 `Bags` / `BagMRU` をバックアップ後にリセットする C# / .NET 8 / WinForms GUI ツールです。

## 対象OS

- Windows 11 x64
- Visual Studio 2026 または .NET 8 SDK
- 発行形式: win-x64 / 自己完結 / 単一EXE
- 管理者権限は原則不要です。UAC昇格は要求しません。

## 何を修復するか

現在ユーザー `HKCU` の以下の Explorer 表示記憶を対象にします。

- `HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags`
- `HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU`
- `HKCU\Software\Microsoft\Windows\Shell\Bags`
- `HKCU\Software\Microsoft\Windows\Shell\BagMRU`
- `HKCU\Software\Microsoft\Windows\ShellNoRoam\Bags`
- `HKCU\Software\Microsoft\Windows\ShellNoRoam\BagMRU`

また、以下に `BagMRU Size` を `REG_DWORD` で設定します。

- `HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell`
- `HKCU\Software\Microsoft\Windows\Shell`

v1.1.0 では `フォルダー種類自動判定を無効化する` 機能を追加しました。チェックON時は以下に `FolderType=NotSpecified` を `REG_SZ` で設定し、画像、音楽、動画フォルダなどの自動テンプレート判定を抑制します。

- `HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell`

このチェックボックスはデフォルトONです。必要ない場合はチェックOFFにできます。チェックOFF時は `FolderType=NotSpecified` の設定をスキップし、既存の `AllFolders\Shell` や `FolderType` は削除しません。

## 何を削除しないか

- 既存のファイルやフォルダ本体は削除しません。
- `HKLM` は操作しません。
- 任意のユーザー入力レジストリキーは削除しません。削除対象キーはコード内で固定されています。
- PowerShellスクリプトを内部実行するラッパーではありません。
- 外部通信、テレメトリ、更新チェックは行いません。

## 使い方

1. `Explorer_FolderView_Reset_Tool_v1.1.0.exe` を起動します。
2. 必要に応じて `BagMRU Size` とバックアップ先を変更します。
3. 必要に応じて `フォルダー種類自動判定を無効化する` のON/OFFを選択します。
4. `事前チェック` で対象キー、現在値、`FolderType` 状態、Explorer起動状態、バックアップ先を確認します。
5. `バックアップのみ実行` で `.reg` バックアップだけを作成できます。
6. `修復を実行` でバックアップ、Explorer停止、対象キー削除、`BagMRU Size` 設定、必要に応じた `FolderType=NotSpecified` 設定、Explorer再起動を実行します。
7. 処理後、任意のフォルダで表示形式を変更し、Explorerを閉じて再度開き、表示形式が保存されるか確認します。

## バックアップ先

初期値は以下です。

```text
%USERPROFILE%\Desktop\Explorer_FolderView_Backup
```

実行ごとに以下の形式のサブフォルダを作成します。

```text
Explorer_FolderView_Backup\yyyyMMdd_HHmmss\
```

ログは UTF-8 BOM 付きでバックアップフォルダへ保存されます。

バックアップ `.reg`、ログ、発行成果物は公開リポジトリに含めないでください。

## 復元方法

1. `復元` ボタンを押します。
2. バックアップ日時フォルダを選択します。
3. 強い確認ダイアログで内容を確認して実行します。
4. `.reg` ファイルを `reg.exe import` でインポートします。
5. 復元後は Explorer の再起動、またはサインアウト/Windows再起動後に確認してください。

## トラブル時の確認

- ログ欄と保存されたログファイルで、対象キーごとの成功/失敗を確認してください。
- 主要キー `HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell` の `BagMRU Size` 設定失敗はエラー扱いです。
- 互換キー `HKCU\Software\Microsoft\Windows\Shell` は Registry API で失敗した場合、`reg.exe add` で再試行します。
- `フォルダー種類自動判定を無効化する` がONの場合、ログに `FolderType=NotSpecified` の設定結果が出力されます。
- Windows再起動は通常不要ですが、反映が不安定な場合や改善しない場合はサインアウト、またはWindows再起動を推奨します。
- Explorer拡張、Windhawk、ExplorerPatcher、StartAllBack、QTTabBar などが Explorer の表示記憶に干渉する場合があります。

## ビルドと発行

```powershell
.\build_publish_win-x64.ps1
```

成果物は `publish\win-x64-single-exe` に出力されます。

スクリプトは以下を順番に実行します。

- `dotnet restore`
- `dotnet build -c Release`
- `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true`

## ライセンス

本ソフトウェアは独自ライセンスです。ソースコードは透明性確保と動作内容確認のために公開されていますが、いわゆるオープンソースソフトウェアではありません。

利用条件、禁止事項、免責事項は [LICENSE](LICENSE) を確認してください。

## 作者

Sunazuri Syuzouin Yukyu
