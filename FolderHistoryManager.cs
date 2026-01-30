using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ColorIt
{
    public class ColoredFolderInfo
    {
        public string Path { get; set; } = "";
        public string ColorHex { get; set; } = "";
        public DateTime ColoredDate { get; set; }
    }

    public static class FolderHistoryManager
    {
        private static readonly string _historyPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ColorIt",
            "history.json"
        );

        private static List<ColoredFolderInfo> _history = new();

        static FolderHistoryManager()
        {
            LoadHistory();
        }

        private static void LoadHistory()
        {
            try
            {
                if (File.Exists(_historyPath))
                {
                    var json = File.ReadAllText(_historyPath);
                    _history = JsonSerializer.Deserialize<List<ColoredFolderInfo>>(json) ?? new();
                }
            }
            catch
            {
                _history = new();
            }
        }

        private static void SaveHistory()
        {
            try
            {
                var dir = System.IO.Path.GetDirectoryName(_historyPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var json = JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_historyPath, json);
            }
            catch { }
        }

        public static void AddOrUpdate(string folderPath, Color color)
        {
            var existing = _history.FirstOrDefault(h => 
                h.Path.Equals(folderPath, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.ColorHex = ColorTranslator.ToHtml(color);
                existing.ColoredDate = DateTime.Now;
            }
            else
            {
                _history.Insert(0, new ColoredFolderInfo
                {
                    Path = folderPath,
                    ColorHex = ColorTranslator.ToHtml(color),
                    ColoredDate = DateTime.Now
                });
            }

            // Keep only last 50 entries
            if (_history.Count > 50)
            {
                _history = _history.Take(50).ToList();
            }

            SaveHistory();
        }

        public static void Remove(string folderPath)
        {
            _history.RemoveAll(h => h.Path.Equals(folderPath, StringComparison.OrdinalIgnoreCase));
            SaveHistory();
        }

        public static List<ColoredFolderInfo> GetHistory()
        {
            // Clean up non-existing folders
            var validHistory = _history.Where(h => Directory.Exists(h.Path)).ToList();
            
            if (validHistory.Count != _history.Count)
            {
                _history = validHistory;
                SaveHistory();
            }

            return _history.ToList();
        }

        public static Color? GetFolderColor(string folderPath)
        {
            var info = _history.FirstOrDefault(h => 
                h.Path.Equals(folderPath, StringComparison.OrdinalIgnoreCase));

            if (info != null)
            {
                try
                {
                    return ColorTranslator.FromHtml(info.ColorHex);
                }
                catch { }
            }

            return null;
        }
    }
}
