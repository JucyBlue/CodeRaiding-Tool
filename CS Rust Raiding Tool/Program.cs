using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;


class InterceptKeys {
    // Constants used in the program
    //yes
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    // Declare the LowLevelKeyboardProc delegate type.
    private static LowLevelKeyboardProc _proc = HookCallback;

    // Declare the keyboard hook handle
    private static IntPtr _hookID = IntPtr.Zero;

    private static int codeCount;
    [DllImport("kernel32.dll")]
    static extern bool SetConsoleTitle(string lpConsoleTitle);

    public static void Main() {

        SetConsoleTitle("#1 best selling code raiding tool in rust");
        Console.Write("(Enter 1 to start with with most common)\nAt what count do you want to start entering codes:");
        codeCount = Int32.Parse(Console.ReadLine());
        Console.WriteLine("\n\n(NUMPAD [\"ENTER\"~write code | \"-\" ~goto prev code | \"+\" ~jump to next code])\nLast code Entered:" + GetLineFromCodes(codeCount-1) + "\nListening...");
        // Set the keyboard hook
        _hookID = SetHook(_proc);

        // Run the application loop
        Application.Run();

        // Unhook the keyboard hook
        UnhookWindowsHookEx(_hookID);

    }

    // Sets the Windows keyboard hook
    private static IntPtr SetHook(LowLevelKeyboardProc proc) {
        // Get the current process and module
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule) {
            // Set the Windows keyboard hook
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    // Delegate for handling low level keyboard input
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    // Callback function for handling low level keyboard input
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
        // If a key is being pressed
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) {
            // Read the virtual key code from the parameter
            int vkCode = Marshal.ReadInt32(lParam);

            if((Keys)vkCode == Keys.Enter){
                
                Console.WriteLine("[" + codeCount.ToString().PadLeft(4, '0') + "]Entering Key:" + GetLineFromCodes(codeCount));
                
                SendKeys.SendWait(GetLineFromCodes(codeCount));
                codeCount++;

            }
            if ((Keys)vkCode == Keys.Add) {
                codeCount++;
                Console.WriteLine("|JUMP| Next Code:" + GetLineFromCodes(codeCount));
            }
            if ((Keys)vkCode == Keys.Subtract) {
                codeCount--;
                Console.WriteLine("|BACK| Next Code:" + GetLineFromCodes(codeCount));
            }
        }

        // Call the next hook in the chain
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    static string GetLineFromCodes(int lineNumber) {
        string line = File.ReadLines("codes.txt").Skip(lineNumber - 1).FirstOrDefault(); 
        return line;
    }


    // Imports required Windows API functions
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
