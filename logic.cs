using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Media;
using System.Globalization;

public class KeyPressListener
{
    // Sound file locations
    private static readonly string hebrewPath = "sounds\\Hebrew.wav";
    private static readonly string englishPath = "sounds\\English.wav";
    private static readonly string IDKPath = "sounds\\IDK.wav";
    
    // Record the time of the last key press
    private static long lastKeyPressTime = 0;
    // Define the delay in milliseconds (initially 2 seconds)
    private static long delay = (long)TimeSpan.FromSeconds(2).TotalMilliseconds;
    // Save the last language we used, if changed we will ignore delay
    private static string lastLanguage = GetCurrentKeyboardLayout();
    // Hook for capturing keyboard events
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    // Indicates whether the listener is currently running.
    public bool IsRunning { get; private set; } = false;

    public void SetDelay(double newDelay)
    {
        delay = (long)newDelay;
    }

    public void Start()
    {
        if (!IsRunning)
        {
            Console.WriteLine("Key Press Listener started.");
            _hookID = SetHook(_proc);

            // Check if the hook was set successfully
            if (_hookID == IntPtr.Zero)
            {
                Console.WriteLine("Failed to set hook.");
                return;
            }
            else
            {
                Console.WriteLine("Hook set successfully.");
                IsRunning = true;
            }
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            UnhookWindowsHookEx(_hookID);
            Console.WriteLine("Unhooked and exiting.");
            IsRunning = false;
        }
    }

    // Sets a low-level keyboard hook to capture key press events.
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            // Set the hook and return the hook ID
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    // Delegate for the keyboard hook
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    // Callback function that handles keyboard events and plays sounds based on the current keyboard layout.
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Console.WriteLine($"Key down: {(Keys)vkCode}");
            long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // Get the current keyboard layout
            string currLanguage = GetCurrentKeyboardLayout();

            if (currentTime - lastKeyPressTime >= delay || lastLanguage != currLanguage)
            {
                //check what language is currently being used and play the corresponding sound
                if (currLanguage == "English" || currLanguage == "United States" || currLanguage == "אנגלית (ארצות הברית)")
                    PlaySound(englishPath);
                else if(currLanguage == "Hebrew" || currLanguage=="עברית" || currLanguage=="עברית (ישראל)")
                    PlaySound(hebrewPath);
                else
                    PlaySound(IDKPath);
                // Update the last key press time
                lastKeyPressTime = currentTime;
                // Update the last language
                lastLanguage = currLanguage; 
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private static void PlaySound(string languagePath)
    {
        try
        {
            using (SoundPlayer player = new SoundPlayer(languagePath))
            {
                player.Play();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error with playing sound: " + e.Message);
        }
    }

    // Retrieves the current keyboard layout as a display name.
    private static string GetCurrentKeyboardLayout()
    {
        IntPtr foregroundWindow = GetForegroundWindow();
        uint processId;
        uint threadId = GetWindowThreadProcessId(foregroundWindow, out processId);
        IntPtr layout = GetKeyboardLayout(threadId);
        int layoutInt = layout.ToInt32() & 0xFFFF;
        return new System.Globalization.CultureInfo(layoutInt).DisplayName;
    }



    // Windows API constants, they are used so i can listen to the kyboard events and understand the current language.
    // Constant for setting a low-level keyboard hook.
    private const int WH_KEYBOARD_LL = 13;
    // Constants for keyboard events.
    private const int WM_KEYDOWN = 0x0100;

    // Installs a hook procedure for keyboard events.
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    // Removes a previously installed hook procedure.
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    // Passes the hook information to the next hook procedure.
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    // Retrieves a module handle for the specified module.
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    // Retrieves the keyboard layout for the specified thread.
    [DllImport("user32.dll")]
    static extern IntPtr GetKeyboardLayout(uint idThread);

    // Retrieves the process ID of the process that created the specified window.
    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    // Retrieves the handle to the window that has the focus.
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

}
