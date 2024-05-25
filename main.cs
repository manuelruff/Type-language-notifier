using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

class KeyPressListener
{
    // Sound file locations
    private static readonly string hebrewPath = "sounds\\Hebrew.wav";
    private static readonly string englishPath = "sounds\\English.wav";
    // Record the time of the last key press
    private static long lastKeyPressTime = 0;
    // Define the delay in milliseconds (30 seconds)
    private static readonly long delay = TimeSpan.FromSeconds(30).Milliseconds;

    static void Main(string[] args)
    {
        Console.WriteLine("Key Press Listener started. Press any key to exit.");
        // Gets the current input language.
        InputLanguage myCurrentLanguage = InputLanguage.CurrentInputLanguage;

        if(myCurrentLanguage != null) 
            Console.WriteLine("Layout: " + myCurrentLanguage.LayoutName);
        else
            Console.WriteLine("There is no current language");        

        // Start listening for key presses
        HookKeyboard();

        // Keep the application running
        Console.ReadLine();
    }


    /**
     * Plays a sound from the specified file location.
     *
     * @param language the file location of the sound to be played
     */
    public static void PlaySound(string languagePath)
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

    public static void HookKeyboard()
    {
        // Create a new keyboard hook
        KeyboardHook hook = new KeyboardHook();
        // Register the key down event handler
        hook.KeyDown += (sender, e) =>
        {
            long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            // Check if enough time has passed since the last key press
            if (currentTime - lastKeyPressTime >= delay)
            {
                // check what language is currently selected
                InputLanguage myCurrentLanguage = InputLanguage.CurrentInputLanguage;
                string language = myCurrentLanguage.LayoutName;
                if(language == "ארצות הברית" || language == "United States")
                    PlaySound(englishPath);
                else
                    PlaySound(hebrewPath);   
                PlaySound(language);
                // Update the last key press time
                lastKeyPressTime = currentTime;
            }
        };
        // Install the keyboard hook
        hook.Install();
    }
}

class KeyboardHook : IDisposable
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private LowLevelKeyboardProc _proc;
    private IntPtr _hookID = IntPtr.Zero;

    public event EventHandler<KeyEventArgs> KeyDown;

    public KeyboardHook()
    {
        _proc = HookCallback;
    }

    public void Install()
    {
        _hookID = SetHook(_proc);
    }

    public void Dispose()
    {
        UnhookWindowsHookEx(_hookID);
    }

    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
        using (var curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            KeyEventArgs args = new KeyEventArgs((Keys)vkCode);
            KeyDown?.Invoke(this, args);
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
