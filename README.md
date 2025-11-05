GLUE — WPF Workflow Tool

GLUE is a desktop app for creating and managing generic workflows to complete multi-step tasks efficiently. Workflows consist of tasks, each with sub-steps that can be web pages (Jira, GitHub, Confluence) or documents.

Features

- **Navbar**: Load Workflow, Create Workflow, Share Workflow buttons.
- **Collapsible Sidebar**: Hierarchical tree view showing Workflows > Tasks > Steps. Each task/step has a progress indicator.
- **View Area**: Split into 4 panels (2x2 grid), each displaying a web page or document via WebView2.
- **Interactions**:
  - Click a step in the sidebar to open it in the top-left view panel.
  - Use "Open All Tasks" button to load the first 4 steps across all panels.
  - Sidebar collapses to a small gutter (48px) for more view space.
- **Workflow Management**: Save/load workflows as JSON files. Share by copying JSON to clipboard.

UI Design

- Inspired by Figma: Clean, modern look with primary red (#d71e28), accent yellow (#ffcd41), light background (#fafafa).
- Aesthetic: Rounded elements, subtle shadows, responsive layout.

Requirements

- Windows OS (WPF targets windows only)
- .NET 9.0 SDK (download from https://dotnet.microsoft.com/download/dotnet/9.0)
- WebView2 runtime (install from https://developer.microsoft.com/en-us/microsoft-edge/webview2/)

How to Run

Since this is a WPF app, it must run on Windows. If you're on macOS/Linux, zip the entire `learn-wpf` folder and transfer to a Windows machine.

On Windows:

1. Ensure .NET 9.0 SDK is installed: `dotnet --version` should show 9.x.x
2. Install WebView2 runtime if not present (Edge browser usually includes it)
3. Open command prompt in the `learn-wpf` folder
4. Build: `dotnet build TestAvalonia.csproj`
5. Run: `dotnet run --project TestAvalonia.csproj`

How to Build and Distribute

To create a standalone executable for Windows:

1. On a machine with .NET 9 SDK: `dotnet publish -c Release -r win-x64 --self-contained`
2. This creates `bin/Release/net9.0-windows/win-x64/publish/`
3. Zip the entire `publish` folder and send to Windows machines.

On the target Windows machine:

- Ensure WebView2 runtime is installed (https://developer.microsoft.com/en-us/microsoft-edge/webview2/)
- Run `TestAvalonia.exe` from the unzipped folder.

No .NET SDK needed on target machines.

Notes

- Sample workflow loaded on startup.
- Progress bars are visual; update logic can be added for real tracking.
- For documents, WebView2 can load local files if URLs point to them.

Future Enhancements

- Edit workflows in UI.
- Drag-and-drop steps.
- Integration with specific tools.
