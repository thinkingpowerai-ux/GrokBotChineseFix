# Grok Bot 中文逗號快捷鍵修正

<img src="assets/robot-icon.png" alt="GrokBotChineseFix 機器人圖示" width="88">

Windows 上的 Grok Bot 桌面版把 `Ctrl + ,` 用來開啟設定。這個小工具只在 Grok Bot 視窗位於前景時攔截該組合鍵，改為輸入全形逗號 `，`。其他程式的 `Ctrl + ,` 維持原本功能。

## 下載後使用

1. 下載 [最新 Release](../../releases/latest) 的 `GrokBotChineseFix.exe`，放在自己選定的資料夾。
2. 雙擊執行。控制視窗顯示「運作中」即已啟用。請保持程式執行。
3. 在 Grok Bot 的文字輸入框按 `Ctrl + ,`，應輸入 `，`，且不開啟設定。

需要 Windows 10/11 與 .NET Framework 4.x；不需要安裝 AutoHotkey。執行檔未簽章，Windows 可能顯示下載檔案的安全提示。原始碼在本儲存庫的 `GrokBotChineseFix.cs`。

按控制視窗的「暫停快捷鍵」可暫停攔截，再按一次恢復；按「結束程式」或關閉視窗即可停止。再次雙擊執行檔會顯示已開啟的控制視窗。

## 登入後自動執行

按 `Win + R`，輸入 `shell:startup`，在開啟的資料夾中建立 `GrokBotChineseFix.exe` 的捷徑。請保留原執行檔在原位。要取消自動執行，刪除該捷徑即可；要完全移除，先結束程式，再刪除執行檔。

## AutoHotkey v2 版本

若已安裝 [AutoHotkey v2](https://www.autohotkey.com/)，也可改用 `GrokBotChineseFix.ahk`。雙擊腳本執行；從通知區的 AutoHotkey 圖示退出。不要同時執行 `.exe` 和 `.ahk`，以免重複攔截。

## 運作方式與原始碼

離線執行檔讀取前景視窗所屬程序的執行檔名稱，僅在名稱為 `Grok Bot.exe` 時攔截 `Ctrl + ,`，並以 Windows Unicode `SendInput` 輸入 U+FF0C。它不依賴 Electron/Chromium 視窗標題、視窗類別或固定安裝路徑，也不修改 Grok Bot、Windows 鍵盤配置或登錄檔。

在 Windows 命令提示字元執行 `build.cmd`，可使用系統的 .NET Framework C# 編譯器從 `GrokBotChineseFix.cs` 重建執行檔。本專案與 Grok Bot 及 xAI 無關。

圖示原始繪製程式位於 `assets/generate_icon.py`，使用 Python 與 Pillow 重建 PNG、ICO。一般使用者只需下載 EXE。

## English

On Windows, this utility turns `Ctrl + ,` into the fullwidth comma `，` only while the Grok Bot desktop window is in the foreground. Download `GrokBotChineseFix.exe` from the latest Release and double-click it. The window lets you pause or exit. Other apps keep their normal shortcut behavior. Windows 10/11 and .NET Framework 4.x are required. For automatic startup, put a shortcut to the EXE in `shell:startup`. The AutoHotkey v2 script is an optional alternative; do not run both versions together.
