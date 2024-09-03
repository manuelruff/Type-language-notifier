
# Type Language Notifier

Type Language Notifier is a Windows application that listens for key presses and plays a corresponding sound based on the current keyboard layout

## Features

- Listens for key presses and detects the current keyboard layout.
- Plays different sounds based on the detected keyboard layout.
- Simple GUI to start and stop the key press listener.
- Adjustable delay interval between sounds.

## Getting Started

### Usage

clone the repository

To view the code, open it with Visual Studio.

To use without accessing the code, navigate to Type-language-notifier\bin\Debug\net6.0-windows and run TypeLanguageNotifierControl.exe.

## Project Structure

- `main.cs`: Entry point for the application. Initializes the Windows Forms application.
- `gui.cs`: Contains the `ControlForm` class, which defines the GUI and its interactions.
- `logic.cs`: Contains the `KeyPressListener` class, which implements the key press listening and sound playing logic.

## to support other languages

To add support for additional languages, download a WAV sound file for the desired language and place it in the 'sounds' directory. Then, add a new case statement in the HookCallback function in logic.cs for the new language.

## Contact

For any questions or suggestions, feel free to open an issue or contact me at [manuel42994299@gmail.com](mailto:manuel42994299@gmail.com).
