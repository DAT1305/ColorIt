# ColorIt - Windows Folder Colorizer 🎨

A Windows application to change folder colors via right-click context menu.

## Features

- ✅ Change folder color with a single right-click
- ✅ 12+ preset colors (Red, Orange, Yellow, Green, Blue, Purple, etc.)
- ✅ Custom color picker for unlimited color choices
- ✅ Reset folder to default color
- ✅ **Auto-refresh** - Changes apply instantly without pressing F5
- ✅ **Bilingual** - Supports both Vietnamese and English
- ✅ **Folder History** - Track all colored folders in one place
- ✅ Full integration with Windows Explorer Context Menu

## System Requirements

- Windows 10/11
- .NET 8.0 Runtime (or use the self-contained version)
- Administrator privileges (for context menu installation)

## Quick Installation

1. Open the `publish` folder
2. **Right-click** on `install.bat` → **Run as administrator**
3. Done! You can now right-click any folder to change its color

## Build from Source

```powershell
# Clone or download source
cd ColorIt

# Build
dotnet build

# Publish (self-contained)
dotnet publish -c Release -r win-x64 --self-contained -o publish
```

## Uninstallation

**Right-click** on `uninstall.bat` → **Run as administrator**

Or run:
```powershell
ColorIt.exe --uninstall
```

## How to Use

1. **Right-click** on any folder in Windows Explorer
2. Select **"🎨 Change Folder Color"**
3. Choose a color from 12 presets or pick a custom color
4. The folder color changes **instantly**!

### Reset to Default
- Right-click on folder → "🎨 Change Folder Color" → "↩️ Reset to default"

### View Colored Folders History
- Open ColorIt app → Click "📋 Colored Folders" to see all folders you've colored

### Change Language
- Open ColorIt app → Select "VI" or "EN" from the dropdown in top-right corner

## How It Works

ColorIt works by:
1. Creating a custom icon file (`folder.ico`) with your chosen color
2. Creating a `desktop.ini` file to make Windows use the custom icon
3. Setting the System attribute on the folder to enable custom icons
4. Auto-refreshing all Explorer windows so changes appear instantly

All files are automatically hidden and don't affect folder contents.

## Screenshots

### Main Window
![Main Window](screenshots/main.png)

### Color Picker
![Color Picker](screenshots/picker.png)

### Context Menu
![Context Menu](screenshots/contextmenu.png)

*(Screenshots will be added later)*

## Troubleshooting

### Icon not changing?
- The app auto-refreshes Explorer, but if it doesn't work:
  - Press F5 to refresh Windows Explorer
  - Close and reopen the folder
  - Restart Windows Explorer

### Context menu not showing?
- Make sure you ran `install.bat` as Administrator
- Restart Windows Explorer or restart your computer

### "Access Denied" error?
- Make sure ColorIt.exe is not blocked by antivirus
- Run the app as Administrator

## Project Structure

```
ColorIt/
├── Program.cs              # Entry point
├── MainForm.cs             # Main window with install/uninstall buttons
├── ColorPickerForm.cs      # Color selection dialog
├── HistoryForm.cs          # Colored folders history view
├── FolderColorizer.cs      # Core logic for changing folder colors
├── ContextMenuManager.cs   # Registry operations for context menu
├── LanguageManager.cs      # Bilingual support (VI/EN)
├── FolderHistoryManager.cs # Track colored folders
└── publish/                # Published executable
    ├── ColorIt.exe
    ├── install.bat
    └── uninstall.bat
```

## License

MIT License

---

Made with ❤️ for Windows users
