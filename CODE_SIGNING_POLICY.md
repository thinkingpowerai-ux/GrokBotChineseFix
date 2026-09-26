# Code signing policy

GrokBotChineseFix is applying for free code signing provided by SignPath.io, certificate by SignPath Foundation. Approval has not been granted. The current v1.0.1 release is unsigned. Do not treat any existing download as SignPath-signed.

The maintainer and release approver is the GitHub repository owner, [thinkingpowerai-ux](https://github.com/thinkingpowerai-ux). Contributions from other people require review by the maintainer before merging. A future signing request will require the maintainer's approval and an artifact built from this public repository by GitHub Actions.

Privacy: The program does not collect, store, or transmit keystrokes or other user data to any networked system. It observes keyboard events locally to intercept `Ctrl + ,` only when the foreground process is `Grok Bot.exe`, then inputs U+FF0C (`，`). It does not change Windows keyboard settings or install a service.

To uninstall, exit the program, remove its shortcut or launcher from `shell:startup` if one was created, and delete the executable. No administrator privileges are required.
