using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;

namespace ColorIt
{
    public partial class ColorPickerForm : Form
    {
        private readonly string _folderPath;
        private readonly Color[] _presetColors = new Color[]
        {
            Color.FromArgb(231, 76, 60),    // Red
            Color.FromArgb(230, 126, 34),   // Orange
            Color.FromArgb(241, 196, 15),   // Yellow
            Color.FromArgb(46, 204, 113),   // Green
            Color.FromArgb(26, 188, 156),   // Teal
            Color.FromArgb(52, 152, 219),   // Blue
            Color.FromArgb(155, 89, 182),   // Purple
            Color.FromArgb(236, 240, 241),  // Light Gray
            Color.FromArgb(149, 165, 166),  // Gray
            Color.FromArgb(52, 73, 94),     // Dark Blue
            Color.FromArgb(241, 148, 138),  // Pink
            Color.FromArgb(133, 193, 233),  // Light Blue
        };

        public ColorPickerForm(string folderPath)
        {
            _folderPath = folderPath;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = LanguageManager.SelectColorTitle;
            this.Size = new Size(420, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.TopMost = true;
            this.KeyPreview = true; // Enable keyboard shortcuts
            this.KeyDown += ColorPickerForm_KeyDown;

            // Folder name label
            string folderName = Path.GetFileName(_folderPath);
            var folderLabel = new Label
            {
                Text = $"📁 {folderName}",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true,
                Location = new Point(20, 15),
                MaximumSize = new Size(380, 0)
            };
            this.Controls.Add(folderLabel);

            // Instruction label with shortcuts hint
            var instructionLabel = new Label
            {
                Text = LanguageManager.SelectColorInstruction + " (1-9, 0, -, =)",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, 50)
            };
            this.Controls.Add(instructionLabel);

            // Color buttons panel
            var colorPanel = new Panel
            {
                Location = new Point(20, 80),
                Size = new Size(370, 180),
                BackColor = Color.Transparent
            };
            this.Controls.Add(colorPanel);

            // Shortcut keys for colors: 1-9, 0, -, =
            string[] shortcuts = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" };

            // Create color buttons
            int btnSize = 55;
            int padding = 15;
            int cols = 6;
            var colorNames = LanguageManager.ColorNames;
            
            for (int i = 0; i < _presetColors.Length; i++)
            {
                int row = i / cols;
                int col = i % cols;
                
                string shortcut = i < shortcuts.Length ? shortcuts[i] : "";
                var colorBtn = CreateColorButton(_presetColors[i], $"{colorNames[i]} [{shortcut}]", shortcut);
                colorBtn.Location = new Point(col * (btnSize + padding), row * (btnSize + padding + 15));
                colorBtn.Tag = _presetColors[i];
                colorBtn.Click += ColorBtn_Click;
                colorPanel.Controls.Add(colorBtn);
            }

            // Custom color button
            var customColorBtn = new Button
            {
                Text = LanguageManager.CustomColor + " (P)",
                Size = new Size(170, 40),
                Location = new Point(20, 270),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.FromArgb(50, 50, 50),
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            customColorBtn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            customColorBtn.Click += CustomColorBtn_Click;
            this.Controls.Add(customColorBtn);

            // Reset button
            var resetBtn = new Button
            {
                Text = LanguageManager.ResetDefault + " (R)",
                Size = new Size(170, 40),
                Location = new Point(210, 270),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.FromArgb(50, 50, 50),
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            resetBtn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            resetBtn.Click += ResetBtn_Click;
            this.Controls.Add(resetBtn);

            // Shortcuts hint label
            var shortcutHint = new Label
            {
                Text = "Esc = Close | P = Pick color | R = Reset",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(100, 320)
            };
            this.Controls.Add(shortcutHint);
        }

        private void ColorPickerForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // Map keys to color indices
            int colorIndex = -1;
            switch (e.KeyCode)
            {
                case Keys.D1: case Keys.NumPad1: colorIndex = 0; break;  // Red
                case Keys.D2: case Keys.NumPad2: colorIndex = 1; break;  // Orange
                case Keys.D3: case Keys.NumPad3: colorIndex = 2; break;  // Yellow
                case Keys.D4: case Keys.NumPad4: colorIndex = 3; break;  // Green
                case Keys.D5: case Keys.NumPad5: colorIndex = 4; break;  // Teal
                case Keys.D6: case Keys.NumPad6: colorIndex = 5; break;  // Blue
                case Keys.D7: case Keys.NumPad7: colorIndex = 6; break;  // Purple
                case Keys.D8: case Keys.NumPad8: colorIndex = 7; break;  // Light Gray
                case Keys.D9: case Keys.NumPad9: colorIndex = 8; break;  // Gray
                case Keys.D0: case Keys.NumPad0: colorIndex = 9; break;  // Dark Blue
                case Keys.OemMinus: colorIndex = 10; break;              // Pink (-)
                case Keys.Oemplus: colorIndex = 11; break;               // Light Blue (=)
                case Keys.P: CustomColorBtn_Click(null, EventArgs.Empty); return;  // Pick custom color
                case Keys.R: ResetBtn_Click(null, EventArgs.Empty); return;        // Reset
                case Keys.Escape: this.Close(); return;                            // Close
            }

            if (colorIndex >= 0 && colorIndex < _presetColors.Length)
            {
                ApplyColor(_presetColors[colorIndex]);
            }
        }

        private Button CreateColorButton(Color color, string tooltip, string shortcutKey = "")
        {
            var btn = new Button
            {
                Size = new Size(55, 55),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                Cursor = Cursors.Hand,
                Text = shortcutKey,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = GetContrastColor(color)
            };
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.White;
            
            // Add tooltip
            var toolTip = new ToolTip();
            toolTip.SetToolTip(btn, tooltip);

            // Hover effect
            btn.MouseEnter += (s, e) => {
                btn.FlatAppearance.BorderColor = Color.FromArgb(50, 50, 50);
            };
            btn.MouseLeave += (s, e) => {
                btn.FlatAppearance.BorderColor = Color.White;
            };

            return btn;
        }

        // Get contrasting text color (white or black) based on background
        private static Color GetContrastColor(Color bg)
        {
            double luminance = (0.299 * bg.R + 0.587 * bg.G + 0.114 * bg.B) / 255;
            return luminance > 0.5 ? Color.Black : Color.White;
        }

        private void ColorBtn_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is Color color)
            {
                ApplyColor(color);
            }
        }

        private void CustomColorBtn_Click(object? sender, EventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.FullOpen = true;
                colorDialog.Color = Color.FromArgb(52, 152, 219); // Default blue
                
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    ApplyColor(colorDialog.Color);
                }
            }
        }

        private void ResetBtn_Click(object? sender, EventArgs e)
        {
            try
            {
                FolderColorizer.ResetFolderColor(_folderPath);
                MessageBox.Show(
                    LanguageManager.ColorResetSuccess,
                    LanguageManager.ColorAppliedTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    LanguageManager.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ApplyColor(Color color)
        {
            try
            {
                FolderColorizer.SetFolderColor(_folderPath, color);
                MessageBox.Show(
                    LanguageManager.ColorApplied,
                    LanguageManager.ColorAppliedTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    LanguageManager.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
