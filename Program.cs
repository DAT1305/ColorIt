using System;
using System.Windows.Forms;

namespace ColorIt
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            // Check command line arguments
            if (args.Length > 0)
            {
                string arg = args[0].ToLower();
                
                if (arg == "--install" || arg == "-i")
                {
                    // Install context menu
                    if (ContextMenuManager.Install())
                    {
                        MessageBox.Show(
                            "ColorIt đã được cài đặt thành công!\n\nBạn có thể nhấn chuột phải vào bất kỳ folder nào để đổi màu.",
                            "Cài đặt thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Không thể cài đặt ColorIt.\nVui lòng chạy với quyền Administrator.",
                            "Lỗi cài đặt",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    return;
                }
                else if (arg == "--uninstall" || arg == "-u")
                {
                    // Uninstall context menu
                    if (ContextMenuManager.Uninstall())
                    {
                        MessageBox.Show(
                            "ColorIt đã được gỡ cài đặt thành công!",
                            "Gỡ cài đặt thành công",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Không thể gỡ cài đặt ColorIt.\nVui lòng chạy với quyền Administrator.",
                            "Lỗi gỡ cài đặt",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    return;
                }
                else if (arg == "--color" || arg == "-c")
                {
                    // Color a specific folder
                    if (args.Length > 1)
                    {
                        string folderPath = args[1];
                        
                        // Remove quotes if present
                        folderPath = folderPath.Trim('"');
                        
                        if (System.IO.Directory.Exists(folderPath))
                        {
                            Application.Run(new ColorPickerForm(folderPath));
                        }
                        else
                        {
                            MessageBox.Show(
                                $"Folder không tồn tại:\n{folderPath}",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                    return;
                }
            }

            // No arguments - show main form
            Application.Run(new MainForm());
        }
    }
}
