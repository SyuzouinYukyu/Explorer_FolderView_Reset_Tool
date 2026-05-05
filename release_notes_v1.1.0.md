# Explorer_FolderView_Reset_Tool v1.1.0

## v1.1.0 の概要

Windows 11 Explorer のフォルダ表示記憶をリセットする既存機能に加えて、フォルダー種類の自動判定を抑制する機能を追加しました。

## 新機能

- 「フォルダー種類自動判定を無効化する」チェックボックスを追加
- チェックボックスはデフォルトON
- FolderType=NotSpecified 設定に対応
- Automatic Folder Type Discovery の抑制に対応
- 事前チェックに FolderType 状態確認を追加
- ログに FolderType 設定結果を出力

## 操作対象

```text
HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags\AllFolders\Shell
FolderType = NotSpecified
```

## 本ツールが行わないこと

- HKLM操作
- ユーザーファイル削除
- 外部通信
- テレメトリ
- 自動アップデート確認
- Windows自動再起動

## 再起動について

通常はExplorer再起動で反映されます。改善しない場合はサインアウトまたはWindows再起動を推奨します。

## ライセンス

個人利用のみ許可、改変不許可、再配布不許可、商用利用不可、法人利用不可。

## 免責事項

実行は自己責任です。いかなる損害にも作者は一切責任を負いません。

## 配布ファイル

Explorer_FolderView_Reset_Tool_v1.1.0.zip

## SHA-256

```text
D78AA52C3B616094916B8E47902994E571EF589B5BD8C8468717119A5B96F0E3
```
