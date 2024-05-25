
# Type Language Notifier

Type Language Notifier is a Windows application that listens for key presses and plays a sound based on the current keyboard layout (language). It features a simple GUI that allows you to start and stop the key press listener and set a delay interval between sounds.

## Features

- Listens for key presses and detects the current keyboard layout.
- Plays different sounds based on the detected keyboard layout.
- Simple GUI to start and stop the key press listener.
- Adjustable delay interval between sounds.

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later

### Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/your-username/type-language-notifier.git
   cd type-language-notifier
   ```

2. Restore the project dependencies:
   ```sh
   dotnet restore
   ```

### Usage

1. Build the project:
   ```sh
   dotnet build
   ```

2. Run the application:
   ```sh
   dotnet run --project main.csproj
   ```

3. The GUI will appear. You can set the delay interval (in seconds) and start or stop the key press listener using the buttons provided.

## Project Structure

- `main.cs`: Entry point for the application. Initializes the Windows Forms application.
- `gui.cs`: Contains the `ControlForm` class, which defines the GUI and its interactions.
- `logic.cs`: Contains the `KeyPressListener` class, which implements the key press listening and sound playing logic.


## Contributing

Contributions are welcome! Please fork the repository and submit a pull request for any improvements or bug fixes.

1. Fork the repository.
2. Create a new branch:
   ```sh
   git checkout -b feature-branch
   ```

3. Make your changes.
4. Commit your changes:
   ```sh
   git commit -m "Add some feature"
   ```

5. Push to the branch:
   ```sh
   git push origin feature-branch
   ```

6. Open a pull request.


## Contact

For any questions or suggestions, feel free to open an issue or contact me at [manuel42994299@gmail.com](mailto:manuel42994299@gmail.com).
