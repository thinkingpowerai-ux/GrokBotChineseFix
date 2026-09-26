#Requires AutoHotkey v2.0
#SingleInstance Force

; Only intercept Ctrl+, while the Grok Bot Electron process owns the foreground window.
; Matching by executable name is more stable than matching a changing window title.
#HotIf WinActive("ahk_exe Grok Bot.exe")
^,::SendText "，"
#HotIf
