using System;
using System.Media;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

class KeyPressListener
{
    // Sound file locations
    private static readonly string hebrewPath = "sounds\\Hebrew.wav";
    private static readonly string englishPath = "sounds\\English.wav";
    // Record the time of the last key press
    private static long lastKeyPressTime = 0;
    // Define the delay in milliseconds (30 seconds)
    private static readonly long delay = (long)TimeSpan.FromSeconds(30).TotalMilliseconds;

    private static IKeyboardMouseEvents _hook;

    static void Main(string[] args)
    {
        Console.WriteLine("Key Press Listener started. Press Enter to exit.");

        // Set up the global keyboard hook
        _hook = Hook.GlobalEvents();

        // Subscribe to the KeyDown event
        _hook.KeyDown += KeyDown;

        // Keep the application running
        Console.ReadLine();

        // Unsubscribe from the event and dispose the hook
        _hook.KeyDown -= KeyDown;
        _hook.Dispose();

        Console.WriteLine("Unsubscribed from KeyDown event and disposed the hook.");
    }

    private static void KeyDown(object sender, KeyEventArgs e)
    {
        Console.WriteLine($"Key down: {e.KeyCode}"); // Debug statement to ensure event is triggered

        long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        // Check if enough time has passed since the last key press
        if (currentTime - lastKeyPressTime >= delay)
        {
            // Check what language is currently selected
            InputLanguage myCurrentLanguage = InputLanguage.CurrentInputLanguage;
            string language = myCurrentLanguage.LayoutName;
            if (language == "ארצות הברית" || language == "United States")
                PlaySound(englishPath);
            else
                PlaySound(hebrewPath);

            // Update the last key press time
            lastKeyPressTime = currentTime;
        }
    }

    /**
     * Plays a sound from the specified file location.
     *
     * @param languagePath the file location of the sound to be played
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
}
