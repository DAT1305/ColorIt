using System;
using Microsoft.Win32;
using System.IO;

namespace ColorIt
{
    public static class ContextMenuManager
    {
        private const string MENU_NAME = "ColorIt";
        // Shortcut key: "&C" makes "C" the hotkey (underlined in menu, press C to activate)
        private const string MENU_TEXT = "🎨 Change Folder &Color";
        private const string REGISTRY_PATH = @"Directory\shell\" + MENU_NAME;
        private const string REGISTRY_COMMAND_PATH = REGISTRY_PATH + @"\command";
        
        // Also register for directory background
        private const string REGISTRY_BG_PATH = @"Directory\Background\shell\" + MENU_NAME;
        private const string REGISTRY_BG_COMMAND_PATH = REGISTRY_BG_PATH + @"\command";

        public static bool Install()
        {
            try
            {
                string exePath = GetExecutablePath();
                
                // Register for folder right-click
                using (var key = Registry.ClassesRoot.CreateSubKey(REGISTRY_PATH))
                {
                    if (key == null) return false;
                    
                    key.SetValue("", MENU_TEXT);
                    key.SetValue("Icon", exePath);
                }

                using (var key = Registry.ClassesRoot.CreateSubKey(REGISTRY_COMMAND_PATH))
                {
                    if (key == null) return false;
                    
                    // Command: ColorIt.exe --color "folder_path"
                    // %1 is the selected item path for classic context menu.
                    // %V is the directory path for some shell verbs and newer Explorer.
                    key.SetValue("", $"\"{exePath}\" --color \"%1\" ");
                }

                // Register for directory background right-click as well (optional).
                using (var key = Registry.ClassesRoot.CreateSubKey(REGISTRY_BG_PATH))
                {
                    if (key == null) return true;
                    key.SetValue("", MENU_TEXT);
                    key.SetValue("Icon", exePath);
                }

                using (var key = Registry.ClassesRoot.CreateSubKey(REGISTRY_BG_COMMAND_PATH))
                {
                    if (key == null) return true;
                    // On background, %V is the current folder.
                    key.SetValue("", $"\"{exePath}\" --color \"%V\" ");
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Uninstall()
        {
            try
            {
                // Remove folder context menu
                try
                {
                    Registry.ClassesRoot.DeleteSubKeyTree(REGISTRY_PATH, false);
                }
                catch { }

                // Remove background context menu
                try
                {
                    Registry.ClassesRoot.DeleteSubKeyTree(REGISTRY_BG_PATH, false);
                }
                catch { }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsInstalled()
        {
            try
            {
                using (var key = Registry.ClassesRoot.OpenSubKey(REGISTRY_PATH))
                {
                    return key != null;
                }
            }
            catch
            {
                return false;
            }
        }

        private static string GetExecutablePath()
        {
            // For single-file publish, use Environment.ProcessPath
            string? processPath = Environment.ProcessPath;
            
            if (!string.IsNullOrEmpty(processPath) && processPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                return processPath;
            }
            
            // Fallback to AppContext.BaseDirectory
            string baseDir = AppContext.BaseDirectory;
            string exePath = Path.Combine(baseDir, "ColorIt.exe");
            
            if (File.Exists(exePath))
            {
                return exePath;
            }
            
            return "ColorIt.exe";
        }
    }
}
