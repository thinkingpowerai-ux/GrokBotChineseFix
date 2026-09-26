using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

internal static class GrokBotChineseFix
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;
    private const int VK_CONTROL = 0x11;
    private const int VK_OEM_COMMA = 0xBC;
    private const int INPUT_KEYBOARD = 1;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint KEYEVENTF_UNICODE = 0x0004;
    private const string TargetExeName = "Grok Bot.exe";
    private const string WindowTitle = "Grok Bot 中文逗號修正";

    private static LowLevelKeyboardProc _callback = HookCallback;
    private static IntPtr _hook = IntPtr.Zero;
    private static bool _paused;
    private static bool _suppressCommaUp;
    private static bool _commaHeld;
    private static Mutex _singleInstance;
    private static StatusWindow _window;

    [STAThread]
    private static void Main()
    {
        bool createdNew;
        _singleInstance = new Mutex(true, "GrokBotChineseFix-31AB", out createdNew);
        if (!createdNew)
        {
            IntPtr existing = FindWindow(null, WindowTitle);
            if (existing != IntPtr.Zero)
            {
                ShowWindow(existing, 9);
                SetForegroundWindow(existing);
            }
            else
            {
                MessageBox.Show("修正程式已在背景執行。請稍候，或從工作管理員結束舊執行個體後再開啟。", WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return;
        }

        try
        {
            _hook = SetWindowsHookEx(WH_KEYBOARD_LL, _callback, GetModuleHandle(null), 0);
            if (_hook == IntPtr.Zero)
                throw new InvalidOperationException("Windows 未允許安裝鍵盤攔截器。錯誤碼：" + Marshal.GetLastWin32Error());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _window = new StatusWindow();
            Application.ApplicationExit += OnExit;
            Application.Run(_window);
        }
        catch (Exception ex)
        {
            OnExit(null, EventArgs.Empty);
            MessageBox.Show("程式啟動失敗：" + ex.Message, WindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void SetPaused(bool paused)
    {
        _paused = paused;
        if (_window != null) _window.UpdateStatus(_paused);
    }

    private static void OnExit(object sender, EventArgs e)
    {
        if (_hook != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hook);
            _hook = IntPtr.Zero;
        }
        if (_singleInstance != null)
        {
            _singleInstance.Dispose();
            _singleInstance = null;
        }
    }

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            KBDLLHOOKSTRUCT key = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            int message = wParam.ToInt32();
            bool isDown = message == WM_KEYDOWN || message == WM_SYSKEYDOWN;
            bool isUp = message == WM_KEYUP || message == WM_SYSKEYUP;
            bool isComma = key.vkCode == VK_OEM_COMMA || key.scanCode == 0x33;

            if (isDown && isComma && !_paused && IsControlDown() && IsGrokBotForeground())
            {
                _suppressCommaUp = true;
                if (!_commaHeld)
                {
                    _commaHeld = true;
                    SendFullwidthComma();
                }
                return new IntPtr(1);
            }

            if (isUp && _suppressCommaUp && isComma)
            {
                _suppressCommaUp = false;
                _commaHeld = false;
                return new IntPtr(1);
            }
        }
        return CallNextHookEx(_hook, nCode, wParam, lParam);
    }

    private static bool IsControlDown()
    {
        return (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
    }

    private static bool IsGrokBotForeground()
    {
        IntPtr hwnd = GetForegroundWindow();
        if (hwnd == IntPtr.Zero) return false;
        uint processId;
        GetWindowThreadProcessId(hwnd, out processId);
        try
        {
            using (Process process = Process.GetProcessById((int)processId))
                return String.Equals(Path.GetFileName(process.MainModule.FileName), TargetExeName, StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }

    private static void SendFullwidthComma()
    {
        INPUT[] input = new INPUT[2];
        input[0].type = INPUT_KEYBOARD;
        input[0].U.ki.wScan = 0xFF0C;
        input[0].U.ki.dwFlags = KEYEVENTF_UNICODE;
        input[1].type = INPUT_KEYBOARD;
        input[1].U.ki.wScan = 0xFF0C;
        input[1].U.ki.dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP;
        SendInput((uint)input.Length, input, Marshal.SizeOf(typeof(INPUT)));
    }

    private sealed class StatusWindow : Form
    {
        private readonly Label _status;
        private readonly Button _pause;

        public StatusWindow()
        {
            Text = WindowTitle;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = true;
            ClientSize = new Size(430, 145);

            _status = new Label();
            _status.AutoSize = false;
            _status.Location = new Point(18, 18);
            _status.Size = new Size(395, 42);
            _status.Text = "運作中：只有 Grok Bot 在前景時才攔截 Ctrl + , 並輸入全形逗號「，」。";
            Controls.Add(_status);

            _pause = new Button();
            _pause.Location = new Point(18, 82);
            _pause.Size = new Size(185, 38);
            _pause.Text = "暫停快捷鍵";
            _pause.Click += delegate { SetPaused(!_paused); };
            Controls.Add(_pause);

            Button exit = new Button();
            exit.Location = new Point(225, 82);
            exit.Size = new Size(185, 38);
            exit.Text = "結束程式";
            exit.Click += delegate { Application.Exit(); };
            Controls.Add(exit);
        }

        public void UpdateStatus(bool paused)
        {
            _status.Text = paused
                ? "已暫停：Ctrl + , 會維持原本行為。"
                : "運作中：只有 Grok Bot 在前景時才攔截 Ctrl + , 並輸入全形逗號「，」。";
            _pause.Text = paused ? "恢復快捷鍵" : "暫停快捷鍵";
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT { public uint type; public INPUTUNION U; }
    [StructLayout(LayoutKind.Explicit)]
    private struct INPUTUNION
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
        [FieldOffset(0)] public HARDWAREINPUT hi;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx; public int dy; public uint mouseData; public uint dwFlags; public uint time; public UIntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk; public ushort wScan; public uint dwFlags; public uint time; public UIntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct HARDWAREINPUT { public uint uMsg; public ushort wParamL; public ushort wParamH; }

    [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr module, uint threadId);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool UnhookWindowsHookEx(IntPtr hook);
    [DllImport("user32.dll")] private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int key);
    [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);
    [DllImport("user32.dll", SetLastError = true)] private static extern uint SendInput(uint count, INPUT[] input, int size);
    [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern IntPtr FindWindow(string className, string windowName);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hwnd, int command);
    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr hwnd);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr GetModuleHandle(string moduleName);
}
