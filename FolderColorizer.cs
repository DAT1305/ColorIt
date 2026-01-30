using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace ColorIt
{
    public static class FolderColorizer
    {
        // Shell32 functions for refreshing icons
        [DllImport("shell32.dll")]
        private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetSetFolderCustomSettings(ref SHFOLDERCUSTOMSETTINGS pfcs, string pszPath, uint dwReadWrite);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string? lpszClass, string? lpszWindow);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFOLDERCUSTOMSETTINGS
        {
            public uint dwSize;
            public uint dwMask;
            public IntPtr pvid;
            public string pszWebViewTemplate;
            public uint cchWebViewTemplate;
            public string pszWebViewTemplateVersion;
            public string pszInfoTip;
            public uint cchInfoTip;
            public IntPtr pclsid;
            public uint dwFlags;
            public string pszIconFile;
            public uint cchIconFile;
            public int iIconIndex;
            public string pszLogo;
            public uint cchLogo;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private const int SHCNE_ASSOCCHANGED = 0x08000000;
        private const int SHCNE_UPDATEDIR = 0x00001000;
        private const int SHCNE_UPDATEITEM = 0x00002000;
        private const int SHCNE_ALLEVENTS = 0x7FFFFFFF;
        private const int SHCNF_IDLIST = 0x0000;
        private const int SHCNF_PATH = 0x0005;
        private const int SHCNF_PATHW = 0x0005;
        private const int SHCNF_FLUSH = 0x1000;
        private const int SHCNF_FLUSHNOWAIT = 0x3000;
        
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_COMMAND = 0x0111;
        private const int VK_F5 = 0x74;

        private const uint FCS_FORCEWRITE = 0x00000002;
        private const uint FCSM_ICONFILE = 0x00000010;

        /// <summary>
        /// Set folder color by creating a custom folder icon
        /// </summary>
        public static void SetFolderColor(string folderPath, Color color)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
            }

            // Create colored folder icon
            string iconPath = Path.Combine(folderPath, "folder.ico");
            CreateColoredFolderIcon(color, iconPath);

            // Create desktop.ini file to set custom icon
            string desktopIniPath = Path.Combine(folderPath, "desktop.ini");
            CreateDesktopIni(desktopIniPath, iconPath);

            // Set folder as system folder to enable custom icon
            SetFolderAsSystem(folderPath);

            // Hide the icon and desktop.ini files
            HideFile(iconPath);
            HideFile(desktopIniPath);

            // Save to history
            FolderHistoryManager.AddOrUpdate(folderPath, color);

            // Force refresh the folder icon in Explorer (auto-refresh without F5)
            ForceRefreshFolderIcon(folderPath);
        }

        /// <summary>
        /// Reset folder to default color
        /// </summary>
        public static void ResetFolderColor(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
            }

            string iconPath = Path.Combine(folderPath, "folder.ico");
            string desktopIniPath = Path.Combine(folderPath, "desktop.ini");

            // Remove custom files
            try
            {
                if (File.Exists(iconPath))
                {
                    File.SetAttributes(iconPath, FileAttributes.Normal);
                    File.Delete(iconPath);
                }
            }
            catch { }

            try
            {
                if (File.Exists(desktopIniPath))
                {
                    File.SetAttributes(desktopIniPath, FileAttributes.Normal);
                    File.Delete(desktopIniPath);
                }
            }
            catch { }

            // Remove system attribute from folder
            try
            {
                var attrs = File.GetAttributes(folderPath);
                attrs &= ~FileAttributes.System;
                attrs &= ~FileAttributes.ReadOnly;
                File.SetAttributes(folderPath, attrs);
            }
            catch { }

            // Remove from history
            FolderHistoryManager.Remove(folderPath);

            // Force refresh (auto-refresh without F5)
            ForceRefreshFolderIcon(folderPath);
        }

        /// <summary>
        /// Force refresh folder icon without requiring F5
        /// </summary>
        private static void ForceRefreshFolderIcon(string folderPath)
        {
            try
            {
                // Step 1: Clear Windows icon cache for this folder
                ClearIconCache();

                // Step 2: Multiple SHChangeNotify calls
                IntPtr pathPtr = Marshal.StringToHGlobalUni(folderPath);
                try
                {
                    SHChangeNotify(SHCNE_UPDATEITEM, SHCNF_PATHW | SHCNF_FLUSHNOWAIT, pathPtr, IntPtr.Zero);
                    SHChangeNotify(SHCNE_UPDATEDIR, SHCNF_PATHW | SHCNF_FLUSHNOWAIT, pathPtr, IntPtr.Zero);
                }
                finally
                {
                    Marshal.FreeHGlobal(pathPtr);
                }

                // Step 3: Notify parent folder
                string? parentPath = Path.GetDirectoryName(folderPath);
                if (!string.IsNullOrEmpty(parentPath))
                {
                    IntPtr parentPtr = Marshal.StringToHGlobalUni(parentPath);
                    try
                    {
                        SHChangeNotify(SHCNE_UPDATEDIR, SHCNF_PATHW | SHCNF_FLUSHNOWAIT, parentPtr, IntPtr.Zero);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(parentPtr);
                    }
                }

                // Step 4: Global association change (forces shell to reload icons)
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);

                // Step 5: Touch folder timestamp
                try
                {
                    var dirInfo = new DirectoryInfo(folderPath);
                    dirInfo.LastWriteTime = DateTime.Now;
                }
                catch { }

                // Step 6: Small delay then refresh Explorer windows
                Thread.Sleep(100);
                RefreshAllExplorerWindows();
                
                // Step 7: Another refresh after a moment for stubborn cases
                Thread.Sleep(200);
                SHChangeNotify(SHCNE_ALLEVENTS, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
            }
            catch { }
        }

        /// <summary>
        /// Clear Windows icon cache
        /// </summary>
        private static void ClearIconCache()
        {
            try
            {
                // Method 1: Delete icon cache files (Windows 10/11)
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string iconCachePath = Path.Combine(localAppData, "Microsoft", "Windows", "Explorer");
                
                if (Directory.Exists(iconCachePath))
                {
                    foreach (var file in Directory.GetFiles(iconCachePath, "iconcache*"))
                    {
                        try { File.Delete(file); } catch { }
                    }
                    foreach (var file in Directory.GetFiles(iconCachePath, "thumbcache*"))
                    {
                        try { File.Delete(file); } catch { }
                    }
                }

                // Method 2: Use ie4uinit to rebuild icon cache (fast, no restart needed)
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "ie4uinit.exe",
                        Arguments = "-show",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using var process = Process.Start(psi);
                    process?.WaitForExit(1000);
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Send F5 refresh to all Explorer windows
        /// </summary>
        private static void RefreshAllExplorerWindows()
        {
            try
            {
                // Find all Explorer windows and send refresh command
                EnumWindows((hWnd, lParam) =>
                {
                    var className = new StringBuilder(256);
                    GetClassName(hWnd, className, className.Capacity);
                    
                    string classNameStr = className.ToString();
                    
                    if (classNameStr == "CabinetWClass" || classNameStr == "ExploreWClass")
                    {
                        // Find the ShellTabWindowClass inside
                        IntPtr shellTab = FindWindowEx(hWnd, IntPtr.Zero, "ShellTabWindowClass", null);
                        if (shellTab != IntPtr.Zero)
                        {
                            // Send F5 to the shell tab
                            PostMessage(shellTab, WM_KEYDOWN, (IntPtr)VK_F5, IntPtr.Zero);
                            PostMessage(shellTab, WM_KEYUP, (IntPtr)VK_F5, IntPtr.Zero);
                        }
                        
                        // Also send to main window
                        PostMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_F5, IntPtr.Zero);
                        PostMessage(hWnd, WM_KEYUP, (IntPtr)VK_F5, IntPtr.Zero);
                    }
                    
                    return true;
                }, IntPtr.Zero);

                // Also try refreshing desktop
                IntPtr desktop = FindWindow("Progman", null);
                if (desktop != IntPtr.Zero)
                {
                    PostMessage(desktop, WM_KEYDOWN, (IntPtr)VK_F5, IntPtr.Zero);
                    PostMessage(desktop, WM_KEYUP, (IntPtr)VK_F5, IntPtr.Zero);
                }
            }
            catch { }
        }

        /// <summary>
        /// Create a colored folder icon
        /// </summary>
        private static void CreateColoredFolderIcon(Color color, string outputPath)
        {
            int size = 256;
            
            using (var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb))
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.Clear(Color.Transparent);

                // Draw folder shape
                DrawFolderIcon(g, size, color);

                // Save as ICO
                SaveAsIcon(bitmap, outputPath);
            }
        }

        /// <summary>
        /// Draw a folder icon shape
        /// </summary>
        private static void DrawFolderIcon(Graphics g, int size, Color color)
        {
            float scale = size / 256f;
            
            // Colors
            Color darkColor = DarkenColor(color, 0.3f);
            Color lightColor = LightenColor(color, 0.2f);
            Color shadowColor = Color.FromArgb(50, 0, 0, 0);

            // Folder back (tab part)
            using (var backPath = new GraphicsPath())
            {
                backPath.AddArc(20 * scale, 40 * scale, 30 * scale, 30 * scale, 180, 90);
                backPath.AddLine(35 * scale, 40 * scale, 80 * scale, 40 * scale);
                backPath.AddLine(80 * scale, 40 * scale, 100 * scale, 60 * scale);
                backPath.AddLine(100 * scale, 60 * scale, 230 * scale, 60 * scale);
                backPath.AddArc(220 * scale, 60 * scale, 20 * scale, 20 * scale, 270, 90);
                backPath.AddLine(240 * scale, 70 * scale, 240 * scale, 80 * scale);
                backPath.AddLine(240 * scale, 80 * scale, 20 * scale, 80 * scale);
                backPath.AddLine(20 * scale, 80 * scale, 20 * scale, 55 * scale);
                backPath.CloseFigure();

                using (var brush = new SolidBrush(darkColor))
                {
                    g.FillPath(brush, backPath);
                }
            }

            // Folder front
            using (var frontPath = new GraphicsPath())
            {
                float top = 75 * scale;
                float bottom = 220 * scale;
                float left = 15 * scale;
                float right = 240 * scale;
                float radius = 15 * scale;

                // Rounded rectangle for folder front
                frontPath.AddArc(left, top, radius * 2, radius * 2, 180, 90);
                frontPath.AddArc(right - radius * 2, top, radius * 2, radius * 2, 270, 90);
                frontPath.AddArc(right - radius * 2, bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                frontPath.AddArc(left, bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                frontPath.CloseFigure();

                // Gradient fill
                using (var brush = new LinearGradientBrush(
                    new PointF(0, top),
                    new PointF(0, bottom),
                    lightColor,
                    color))
                {
                    g.FillPath(brush, frontPath);
                }

                // Border
                using (var pen = new Pen(darkColor, 2 * scale))
                {
                    g.DrawPath(pen, frontPath);
                }
            }

            // Highlight line on top
            using (var highlightPen = new Pen(Color.FromArgb(100, 255, 255, 255), 3 * scale))
            {
                g.DrawLine(highlightPen, 30 * scale, 85 * scale, 225 * scale, 85 * scale);
            }
        }

        /// <summary>
        /// Save bitmap as ICO file with multiple sizes
        /// </summary>
        private static void SaveAsIcon(Bitmap source, string outputPath)
        {
            int[] sizes = { 16, 32, 48, 64, 128, 256 };
            
            using (var stream = new FileStream(outputPath, FileMode.Create))
            using (var writer = new BinaryWriter(stream))
            {
                // ICO header
                writer.Write((short)0);      // Reserved
                writer.Write((short)1);      // Type (1 = ICO)
                writer.Write((short)sizes.Length);  // Number of images

                // Calculate header size
                int headerSize = 6 + (sizes.Length * 16);
                int offset = headerSize;
                
                // Store image data temporarily
                var imageData = new byte[sizes.Length][];
                
                for (int i = 0; i < sizes.Length; i++)
                {
                    int size = sizes[i];
                    using (var resized = new Bitmap(source, size, size))
                    using (var ms = new MemoryStream())
                    {
                        resized.Save(ms, ImageFormat.Png);
                        imageData[i] = ms.ToArray();
                    }
                }

                // Write directory entries
                for (int i = 0; i < sizes.Length; i++)
                {
                    int size = sizes[i];
                    writer.Write((byte)(size == 256 ? 0 : size));  // Width
                    writer.Write((byte)(size == 256 ? 0 : size));  // Height
                    writer.Write((byte)0);   // Color palette
                    writer.Write((byte)0);   // Reserved
                    writer.Write((short)1);  // Color planes
                    writer.Write((short)32); // Bits per pixel
                    writer.Write(imageData[i].Length);  // Image size
                    writer.Write(offset);    // Image offset
                    offset += imageData[i].Length;
                }

                // Write image data
                for (int i = 0; i < sizes.Length; i++)
                {
                    writer.Write(imageData[i]);
                }
            }
        }

        /// <summary>
        /// Create desktop.ini file
        /// </summary>
        private static void CreateDesktopIni(string path, string iconFile)
        {
            // Remove existing file if present
            if (File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }

            var content = new StringBuilder();
            content.AppendLine("[.ShellClassInfo]");
            // Use absolute path to avoid cases where Explorer fails to resolve relative paths.
            content.AppendLine($"IconResource={iconFile},0");
            // Add an index to reduce icon caching issues.
            content.AppendLine("IconIndex=0");
            content.AppendLine("[ViewState]");
            content.AppendLine("Mode=");
            content.AppendLine("Vid=");
            content.AppendLine("FolderType=Generic");

            File.WriteAllText(path, content.ToString(), Encoding.Unicode);
        }

        /// <summary>
        /// Set folder as system folder to enable custom icon
        /// </summary>
        private static void SetFolderAsSystem(string folderPath)
        {
            var attrs = File.GetAttributes(folderPath);
            attrs |= FileAttributes.System;
            File.SetAttributes(folderPath, attrs);
        }

        /// <summary>
        /// Hide a file
        /// </summary>
        private static void HideFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var attrs = File.GetAttributes(filePath);
                attrs |= FileAttributes.Hidden;
                attrs |= FileAttributes.System;
                File.SetAttributes(filePath, attrs);
            }
        }

        /// <summary>
        /// Darken a color
        /// </summary>
        private static Color DarkenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R * (1 - factor)),
                (int)(color.G * (1 - factor)),
                (int)(color.B * (1 - factor)));
        }

        /// <summary>
        /// Lighten a color
        /// </summary>
        private static Color LightenColor(Color color, float factor)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R + (255 - color.R) * factor),
                (int)(color.G + (255 - color.G) * factor),
                (int)(color.B + (255 - color.B) * factor));
        }
    }
}
