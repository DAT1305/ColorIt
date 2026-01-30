using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ColorIt
{
    public static class LanguageManager
    {
        public enum Language
        {
            Vietnamese,
            English
        }

        private static Language _currentLanguage = Language.Vietnamese;
        private static readonly string _settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ColorIt",
            "settings.json"
        );

        public static Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                SaveSettings();
            }
        }

        static LanguageManager()
        {
            LoadSettings();
        }

        private static void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if (settings != null && settings.TryGetValue("language", out var lang))
                    {
                        _currentLanguage = lang == "en" ? Language.English : Language.Vietnamese;
                    }
                }
            }
            catch { }
        }

        private static void SaveSettings()
        {
            try
            {
                var dir = Path.GetDirectoryName(_settingsPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var settings = new Dictionary<string, string>
                {
                    ["language"] = _currentLanguage == Language.English ? "en" : "vi"
                };
                var json = JsonSerializer.Serialize(settings);
                File.WriteAllText(_settingsPath, json);
            }
            catch { }
        }

        // Localized strings
        public static string AppTitle => _currentLanguage == Language.English 
            ? "ColorIt - Windows Folder Colorizer" 
            : "ColorIt - Đổi màu Folder Windows";

        public static string Subtitle => _currentLanguage == Language.English
            ? "Windows Folder Colorizer"
            : "Đổi màu Folder Windows";

        public static string Description => _currentLanguage == Language.English
            ? "Change folder colors with just a right-click!"
            : "Đổi màu folder Windows chỉ với một cú click chuột phải!";

        public static string InstallButton => _currentLanguage == Language.English
            ? "📥 Install Context Menu"
            : "📥 Cài đặt Context Menu";

        public static string UninstallButton => _currentLanguage == Language.English
            ? "📤 Uninstall"
            : "📤 Gỡ cài đặt";

        public static string HistoryButton => _currentLanguage == Language.English
            ? "📋 Colored Folders"
            : "📋 Folder đã đổi màu";

        public static string StatusInstalled => _currentLanguage == Language.English
            ? "✅ Status: Installed"
            : "✅ Trạng thái: Đã cài đặt";

        public static string StatusNotInstalled => _currentLanguage == Language.English
            ? "❌ Status: Not installed"
            : "❌ Trạng thái: Chưa cài đặt";

        public static string InstallSuccess => _currentLanguage == Language.English
            ? "ColorIt installed successfully!\n\nNow you can:\n1. Open Windows Explorer\n2. Right-click on any folder\n3. Select '🎨 Change Folder Color'\n4. Pick your color!"
            : "ColorIt đã được cài đặt thành công!\n\nBây giờ bạn có thể:\n1. Mở Windows Explorer\n2. Nhấn chuột phải vào bất kỳ folder nào\n3. Chọn '🎨 Change Folder Color'\n4. Chọn màu bạn thích!";

        public static string InstallSuccessTitle => _currentLanguage == Language.English
            ? "Installation Successful"
            : "Cài đặt thành công";

        public static string InstallError => _currentLanguage == Language.English
            ? "Could not install ColorIt.\n\nPlease ensure:\n- Run the application as Administrator\n- No antivirus is blocking"
            : "Không thể cài đặt ColorIt.\n\nVui lòng đảm bảo:\n- Chạy ứng dụng với quyền Administrator\n- Không có phần mềm antivirus chặn";

        public static string InstallErrorTitle => _currentLanguage == Language.English
            ? "Installation Error"
            : "Lỗi cài đặt";

        public static string UninstallConfirm => _currentLanguage == Language.English
            ? "Are you sure you want to uninstall ColorIt?\n\nThe context menu will be removed from Windows Explorer."
            : "Bạn có chắc muốn gỡ cài đặt ColorIt?\n\nContext menu sẽ bị xóa khỏi Windows Explorer.";

        public static string UninstallConfirmTitle => _currentLanguage == Language.English
            ? "Confirm Uninstall"
            : "Xác nhận gỡ cài đặt";

        public static string UninstallSuccess => _currentLanguage == Language.English
            ? "ColorIt has been uninstalled successfully!"
            : "ColorIt đã được gỡ cài đặt thành công!";

        public static string UninstallSuccessTitle => _currentLanguage == Language.English
            ? "Uninstall Successful"
            : "Gỡ cài đặt thành công";

        public static string UninstallError => _currentLanguage == Language.English
            ? "Could not uninstall ColorIt.\nPlease run as Administrator."
            : "Không thể gỡ cài đặt ColorIt.\nVui lòng chạy với quyền Administrator.";

        public static string UninstallErrorTitle => _currentLanguage == Language.English
            ? "Uninstall Error"
            : "Lỗi gỡ cài đặt";

        public static string SelectColorTitle => _currentLanguage == Language.English
            ? "Select Folder Color"
            : "Chọn màu cho Folder";

        public static string SelectColorInstruction => _currentLanguage == Language.English
            ? "Choose a color for the folder:"
            : "Chọn màu cho folder:";

        public static string CustomColor => _currentLanguage == Language.English
            ? "🎨 Custom color..."
            : "🎨 Màu tùy chọn...";

        public static string ResetDefault => _currentLanguage == Language.English
            ? "↩️ Reset to default"
            : "↩️ Khôi phục mặc định";

        public static string ColorApplied => _currentLanguage == Language.English
            ? "Folder color changed successfully!"
            : "Đã đổi màu folder thành công!";

        public static string ColorAppliedTitle => _currentLanguage == Language.English
            ? "Success"
            : "Thành công";

        public static string ColorResetSuccess => _currentLanguage == Language.English
            ? "Folder color has been reset to default!"
            : "Đã khôi phục màu mặc định cho folder!";

        public static string Error => _currentLanguage == Language.English
            ? "Error"
            : "Lỗi";

        public static string FolderNotFound => _currentLanguage == Language.English
            ? "Folder not found"
            : "Folder không tồn tại";

        public static string HistoryTitle => _currentLanguage == Language.English
            ? "Colored Folders History"
            : "Lịch sử Folder đã đổi màu";

        public static string NoHistory => _currentLanguage == Language.English
            ? "No colored folders yet.\nRight-click on a folder to change its color!"
            : "Chưa có folder nào được đổi màu.\nNhấn chuột phải vào folder để đổi màu!";

        public static string OpenFolder => _currentLanguage == Language.English
            ? "Open"
            : "Mở";

        public static string ResetColor => _currentLanguage == Language.English
            ? "Reset"
            : "Khôi phục";

        public static string RemoveFromList => _currentLanguage == Language.English
            ? "Remove from list"
            : "Xóa khỏi danh sách";

        public static string Footer => _currentLanguage == Language.English
            ? "© 2024 ColorIt - Made with ❤️"
            : "© 2024 ColorIt - Made with ❤️";

        public static string[] ColorNames => _currentLanguage == Language.English
            ? new[] { "Red", "Orange", "Yellow", "Green", "Teal", "Blue", "Purple", "Light Gray", "Gray", "Dark Blue", "Pink", "Light Blue" }
            : new[] { "Đỏ", "Cam", "Vàng", "Xanh lá", "Xanh ngọc", "Xanh dương", "Tím", "Xám nhạt", "Xám", "Xanh đậm", "Hồng", "Xanh nhạt" };
    }
}
