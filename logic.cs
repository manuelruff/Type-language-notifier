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

    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

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
                // todo add all the possibilities for us
                if (currLanguage == "English" || currLanguage == "United States" || currLanguage == "אנגלית (ארצות הברית)")
                    PlaySound(englishPath);
                else if(currLanguage == "Hebrew" || currLanguage=="עברית" || currLanguage=="עברית (ישראל)")
                    PlaySound(hebrewPath);
                else
                    PlaySound(IDKPath);
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
