
//██████╗░███████╗██████╗░░░░░░░██╗░░██╗  ░░░░░██╗██╗████████╗██╗░░░██╗██╗░░██╗
//██╔══██╗██╔════╝██╔══██╗░░░░░░╚██╗██╔╝  ░░░░░██║██║╚══██╔══╝██║░░░██║╚██╗██╔╝
//██████╔╝█████╗░░██║░░██║█████╗░╚███╔╝░  ░░░░░██║██║░░░██║░░░██║░░░██║░╚███╔╝░
//██╔══██╗██╔══╝░░██║░░██║╚════╝░██╔██╗░  ██╗░░██║██║░░░██║░░░██║░░░██║░██╔██╗░
//██║░░██║███████╗██████╔╝░░░░░░██╔╝╚██╗  ╚█████╔╝██║░░░██║░░░╚██████╔╝██╔╝╚██╗
//╚═╝░░╚═╝╚══════╝╚═════╝░░░░░░░╚═╝░░╚═╝  ░╚════╝░╚═╝░░░╚═╝░░░░╚═════╝░╚═╝░░╚═╝



//██████╗░██╗░██████╗░█████╗░░█████╗░██████╗░██████╗░
//██╔══██╗██║██╔════╝██╔══██╗██╔══██╗██╔══██╗██╔══██╗
//██║░░██║██║╚█████╗░██║░░╚═╝██║░░██║██████╔╝██║░░██║
//██║░░██║██║░╚═══██╗██║░░██╗██║░░██║██╔══██╗██║░░██║
//██████╔╝██║██████╔╝╚█████╔╝╚█████╔╝██║░░██║██████╔╝
//╚═════╝░╚═╝╚═════╝░░╚════╝░░╚════╝░╚═╝░░╚═╝╚═════╝░

// https://discord.gg/f7KPc9JyeY









using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LowLevelKeyBind
{
    public partial class Form1 : Form
    {
        // Constants for hook types and Windows messages
        private const int WH_KEYBOARD_LL = 13;       // Low-level keyboard hook
        private const int WH_MOUSE_LL = 14;          // Low-level mouse hook
        private const int WM_KEYDOWN = 0x0100;       // Key down message
        private const int WM_XBUTTONDOWN = 0x020B;   // Extended mouse button down message (Mouse 4 & 5)
        private const int WM_LBUTTONDOWN = 0x0201;   // Left mouse button down
        private const int WM_RBUTTONDOWN = 0x0204;   // Right mouse button down
        private const int WM_MBUTTONDOWN = 0x0207;   // Middle mouse button down

        // Delegates for hook callbacks
        private static LowLevelKeyboardProc _keyboardProc;
        private static LowLevelMouseProc _mouseProc;

        // Hook handles to track current hooks
        private static IntPtr _keyboardHookID = IntPtr.Zero;
        private static IntPtr _mouseHookID = IntPtr.Zero;

        // Variables for tracking the first hotkey (linked to guna2Button1)
        private Keys selectedKey = Keys.None;
        private int selectedMouseButton = -1;
        private bool waitingForKeybind = false;

        // Variables for tracking the second hotkey (linked to guna2Button3)
        private Keys selectedKey2 = Keys.None;
        private int selectedMouseButton2 = -1;
        private bool waitingForKeybind2 = false;

        // Delegate types for hooks
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        public Form1()
        {
            InitializeComponent();

            // Assign hook callback methods
            _keyboardProc = KeyboardHookCallback;
            _mouseProc = MouseHookCallback;

            // Set global keyboard and mouse hooks
            _keyboardHookID = SetGlobalHook(WH_KEYBOARD_LL, _keyboardProc);
            _mouseHookID = SetGlobalHook(WH_MOUSE_LL, _mouseProc);

            // Ensure hooks are removed when the form closes
            this.FormClosed += Form1_FormClosed;
        }

        // Executes when guna2Button1 is clicked
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            label1.Text = "Button clicked!";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Optional form load logic
        }

        // Starts hotkey assignment for guna2Button1
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            label1.Text = "Set hotkey for guna2Button1...";
            guna2Button2.Text = "...";
            selectedKey = Keys.None;
            selectedMouseButton = -1;
            waitingForKeybind = true;
        }

        // Executes when guna2Button3 is clicked
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            label1.Text = "guna2Button3_Click";
        }

        // Executes when guna2Button5 is clicked (e.g., fixed hotkey F6)
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            label1.Text = "guna2Button5_Click";
        }

        // Executes when guna2Button6 is clicked (e.g., fixed hotkey F7)
        private void guna2Button6_Click(object sender, EventArgs e)
        {
            label1.Text = "guna2Button6_Click";
        }

        // Starts hotkey assignment for guna2Button3
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            label1.Text = "Set hotkey for guna2Button3...";
            guna2Button4.Text = "...";
            selectedKey2 = Keys.None;
            selectedMouseButton2 = -1;
            waitingForKeybind2 = true;
        }

        // Handles key or mouse input to assign or trigger hotkeys dynamically
        private void HandleUnifiedHotkey(string hotkeyName, Keys? key = null, int? mouseButton = null)
        {
            // If waiting for first hotkey assignment
            if (waitingForKeybind)
            {
                selectedKey = key ?? Keys.None;
                selectedMouseButton = mouseButton ?? -1;
                waitingForKeybind = false;

                guna2Button2.Invoke((MethodInvoker)(() => guna2Button2.Text = $"Hotkey: {hotkeyName}"));
                label1.Invoke((MethodInvoker)(() => label1.Text = $"Hotkey for guna2Button1 set to: {hotkeyName}"));
                return;
            }

            // If hotkey matches first assigned key or mouse button, trigger button 1
            if ((key.HasValue && selectedKey == key.Value && selectedKey != Keys.None) ||
                (mouseButton.HasValue && selectedMouseButton == mouseButton.Value && selectedMouseButton != -1))
            {
                guna2Button1.Invoke((MethodInvoker)(() => guna2Button1.PerformClick()));
            }

            // If waiting for second hotkey assignment
            if (waitingForKeybind2)
            {
                selectedKey2 = key ?? Keys.None;
                selectedMouseButton2 = mouseButton ?? -1;
                waitingForKeybind2 = false;

                guna2Button4.Invoke((MethodInvoker)(() => guna2Button4.Text = $"Hotkey: {hotkeyName}"));
                label1.Invoke((MethodInvoker)(() => label1.Text = $"Hotkey for guna2Button3 set to: {hotkeyName}"));
                return;
            }

            // If hotkey matches second assigned key or mouse button, trigger button 3
            if ((key.HasValue && selectedKey2 == key.Value && selectedKey2 != Keys.None) ||
                (mouseButton.HasValue && selectedMouseButton2 == mouseButton.Value && selectedMouseButton2 != -1))
            {
                guna2Button3.Invoke((MethodInvoker)(() => guna2Button3.PerformClick()));
            }
        }

        // Callback for low-level keyboard hook
        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                // Handle dynamic key-based hotkeys
                HandleUnifiedHotkey(key.ToString(), key: key);

                // Handle fixed key triggers (F6, F7)
                if (key == Keys.F6)
                {
                    if (guna2Button5.InvokeRequired)
                        guna2Button5.Invoke((MethodInvoker)(() => guna2Button1.PerformClick()));
                    else
                        guna2Button5.PerformClick();
                }
                else if (key == Keys.F7)
                {
                    if (guna2Button6.InvokeRequired)
                        guna2Button6.Invoke((MethodInvoker)(() => guna2Button2.PerformClick()));
                    else
                        guna2Button6.PerformClick();
                }
            }

            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }

        // Callback for low-level mouse hook
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int buttonId = -1;
                string btnName = "";

                // Detect which mouse button was pressed
                switch ((int)wParam)
                {
                    case WM_LBUTTONDOWN: buttonId = 0; btnName = "Left Mouse"; break;
                    case WM_RBUTTONDOWN: buttonId = 1; btnName = "Right Mouse"; break;
                    case WM_MBUTTONDOWN: buttonId = 2; btnName = "Middle Mouse"; break;
                    case WM_XBUTTONDOWN:
                        int mouseData = Marshal.ReadInt32((IntPtr)((long)lParam + 8));
                        int xButton = mouseData >> 16;
                        buttonId = 2 + xButton;
                        btnName = buttonId == 3 ? "Mouse4" : "Mouse5";
                        break;
                }

                // Handle mouse-based dynamic hotkeys
                if (buttonId != -1)
                {
                    HandleUnifiedHotkey(btnName, mouseButton: buttonId);
                }
            }

            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }

        // Set a global Windows hook
        private static IntPtr SetGlobalHook(int hookId, Delegate proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                ProcessModule curModule = curProcess.MainModule;

                IntPtr hook = SetWindowsHookEx(hookId, proc, GetModuleHandle(curModule.ModuleName), 0);
                if (hook == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode, $"Failed to set hook {hookId}");
                }

                return hook;
            }
        }

        // Remove hooks when form is closed
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnhookWindowsHookEx(_keyboardHookID);
            UnhookWindowsHookEx(_mouseHookID);
        }

        // DLL Imports for hook management
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}



