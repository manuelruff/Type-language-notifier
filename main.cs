using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Media;
using System.Globalization;

class KeyPressListener
{
    // Sound file locations
    private static readonly string hebrewPath = "sounds\\Hebrew.wav";
    private static readonly string englishPath = "sounds\\English.wav";
    
    // Record the time of the last key press
    private static long lastKeyPressTime = 0;
    // Define the delay in milliseconds (30 seconds)
    private static readonly long delay = (long)TimeSpan.FromSeconds(2).TotalMilliseconds;
    // Save the last language we used, if changed we will ignore delay
    private static string lastLanguage = InputLanguage.CurrentInputLanguage.LayoutName; 

    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    static void Main(string[] args)
    {
        Console.WriteLine("Key Press Listener started. Press Enter to exit.");
        string lang = InputLanguage.CurrentInputLanguage.Culture.Name;
        Console.WriteLine(InputLanguage.CurrentInputLanguage.LayoutName);
        Console.WriteLine("Current language: " + lang);
        Console.WriteLine(CultureInfo.CurrentCulture.KeyboardLayoutId);
        Console.WriteLine(CultureInfo.CurrentCulture.EnglishName);
        
        Console.WriteLine("switch");
        lang = InputLanguage.CurrentInputLanguage.Culture.Name;
        Console.WriteLine("Current language: " + lang);
        Console.WriteLine(InputLanguage.CurrentInputLanguage.LayoutName);
        Console.WriteLine(CultureInfo.CurrentCulture.KeyboardLayoutId);
        Console.WriteLine(CultureInfo.CurrentCulture.EnglishName);
        _hookID = SetHook(_proc);

        // Print the initial language
        Console.WriteLine("Initial language: " + lastLanguage);

        // Check if the hook was set successfully
        if (_hookID == IntPtr.Zero)
        {
            Console.WriteLine("Failed to set hook.");
            return;
        }
        else
        {
            Console.WriteLine("Hook set successfully.");
        }

        // Use Application.Run to keep the application running efficiently
        Application.Run();

        UnhookWindowsHookEx(_hookID);

        Console.WriteLine("Unhooked and exiting.");
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Console.WriteLine($"Key down: {(Keys)vkCode}");
            long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            // Get the current keyboard layout
            string currLanguage = GetCurrentKeyboardLayout();

            // Debug: Print the current and last language
            Console.WriteLine($"Current language: {currLanguage}");
            Console.WriteLine($"Last language: {lastLanguage}");

            if (currentTime - lastKeyPressTime >= delay || lastLanguage != currLanguage)
            {
                if (currLanguage == "English" || currLanguage == "United States")
                    PlaySound(englishPath);
                else
                    PlaySound(hebrewPath);

                // Update the last key press time
                lastKeyPressTime = currentTime;
                // Update the last language
                lastLanguage = currLanguage; // Ensure lastLanguage is updated
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
    private static string GetCurrentKeyboardLayout()
    {
        IntPtr foregroundWindow = GetForegroundWindow();
        uint processId;
        uint threadId = GetWindowThreadProcessId(foregroundWindow, out processId);
        IntPtr layout = GetKeyboardLayout(threadId);
        int layoutInt = layout.ToInt32() & 0xFFFF;
        return new System.Globalization.CultureInfo(layoutInt).DisplayName;
    }


    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    static extern IntPtr GetKeyboardLayout(uint idThread);

    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();


}
