# WindowShot

A Windows desktop screenshot utility with a background capture service and system tray controls.

## Features

- Capture the active window or the entire screen
- Choose from multiple capture methods
- Configure a keyboard shortcut for capturing screenshots
- Start automatically with Windows
- Control the background capture service from the system tray
- Switch between Window and Screen capture modes

## Requirements

- Windows 10, build 17763 or later
- .NET 9.0 Runtime
- Windows App Runtime 2.5.1 or later

## Installation

### Download

Extract the archive file downloaded from [`Releases`](https://github.com/nfmcpwr/WindowShot/Releases/latest) page

### Build manually(.NET 9.0 SDK is required)

```cmd
git clone https://github.com/nfmcpwr/WindowShot.git
cd WindowShot
dotnet build WindowShot.slnx
```

#### Build output

If you built the project from source, output to the following directory: 

```text
out/Debug
out/Release
```

## Usage

Run `WindowShotService.exe` to start WindowShot.

Press the configured keyboard shortcut to capture a screenshot.

### Change settings

Open the configuration UI from the system tray and modify the desired settings.

## Configuration

WindowShot uses the default settings when no configuration file exists.

<details><summary>Default config values</summary>
  
- `Startup`: `On`
- `Shortcut key`: `IMEConvert`
- `Mode`: `Window`
- `Capture method`: `Bitblt`

</details>

- `Startup`: Specifies whether WindowShot starts automatically with Windows

- `Shortcut key`: Specifies the key used to take a screenshot

- `Mode`: Specify the area to be captured
    - `Window`: Captures the active window
    - `Screen`: Captures the entire screen containing the active window

- `Capture method`: Specifies the method to use for taking screen captures

## License

WindowShot is licensed under the [GNU General Public License 3.0](https://github.com/nfmcpwr/WindowShot/blob/master/LICENSE)
