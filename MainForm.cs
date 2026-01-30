using System;
using System.Drawing;
using System.Windows.Forms;

namespace ColorIt
{
    public partial class MainForm : Form
    {
        private Label? _statusLabel;
        private Button? _installBtn;
        private Button? _uninstallBtn;
        private Button? _historyBtn;
        private Label? _titleLabel;
        private Label? _subtitleLabel;
        private Label? _descLabel;
        private ComboBox? _langCombo;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = LanguageManager.AppTitle;
            this.Size = new Size(500, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Language selector
            var langLabel = new Label
            {
                Text = "🌐",
                Font = new Font("Segoe UI", 14),
                AutoSize = true,
                Location = new Point(400, 15)
            };
            this.Controls.Add(langLabel);

            _langCombo = new ComboBox
            {
                Size = new Size(70, 25),
                Location = new Point(420, 15),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            _langCombo.Items.AddRange(new[] { "VI", "EN" });
            _langCombo.SelectedIndex = LanguageManager.CurrentLanguage == LanguageManager.Language.English ? 1 : 0;
            _langCombo.SelectedIndexChanged += LangCombo_SelectedIndexChanged;
            this.Controls.Add(_langCombo);

            // Title Label
            _titleLabel = new Label
            {
                Text = "🎨 ColorIt",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true,
                Location = new Point(150, 50)
            };
            this.Controls.Add(_titleLabel);

            // Subtitle
            _subtitleLabel = new Label
            {
                Text = LanguageManager.Subtitle,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(145, 105)
            };
            this.Controls.Add(_subtitleLabel);

            // Description
            _descLabel = new Label
            {
                Text = LanguageManager.Description,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(90, 150)
            };
            this.Controls.Add(_descLabel);

            // Install Button
            _installBtn = new Button
            {
                Text = LanguageManager.InstallButton,
                Size = new Size(220, 45),
                Location = new Point(140, 200),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                Cursor = Cursors.Hand
            };
            _installBtn.FlatAppearance.BorderSize = 0;
            _installBtn.Click += InstallBtn_Click;
            this.Controls.Add(_installBtn);

            // Uninstall Button
            _uninstallBtn = new Button
            {
                Text = LanguageManager.UninstallButton,
                Size = new Size(220, 45),
                Location = new Point(140, 255),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                Cursor = Cursors.Hand
            };
            _uninstallBtn.FlatAppearance.BorderSize = 0;
            _uninstallBtn.Click += UninstallBtn_Click;
            this.Controls.Add(_uninstallBtn);

            // History Button
            _historyBtn = new Button
            {
                Text = LanguageManager.HistoryButton,
                Size = new Size(220, 45),
                Location = new Point(140, 310),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                Cursor = Cursors.Hand
            };
            _historyBtn.FlatAppearance.BorderSize = 0;
            _historyBtn.Click += HistoryBtn_Click;
            this.Controls.Add(_historyBtn);

            // Status Label
            _statusLabel = new Label
            {
                Text = GetStatusText(),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                Location = new Point(155, 380)
            };
            this.Controls.Add(_statusLabel);

            // Footer
            var footerLabel = new Label
            {
                Text = LanguageManager.Footer,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(175, 420)
            };
            this.Controls.Add(footerLabel);
        }

        private void LangCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_langCombo == null) return;
            
            LanguageManager.CurrentLanguage = _langCombo.SelectedIndex == 1 
                ? LanguageManager.Language.English 
                : LanguageManager.Language.Vietnamese;
            
            UpdateUI();
        }

        private void UpdateUI()
        {
            this.Text = LanguageManager.AppTitle;
            
            if (_subtitleLabel != null)
                _subtitleLabel.Text = LanguageManager.Subtitle;
            
            if (_descLabel != null)
                _descLabel.Text = LanguageManager.Description;
            
            if (_installBtn != null)
                _installBtn.Text = LanguageManager.InstallButton;
            
            if (_uninstallBtn != null)
                _uninstallBtn.Text = LanguageManager.UninstallButton;
            
            if (_historyBtn != null)
                _historyBtn.Text = LanguageManager.HistoryButton;
            
            UpdateStatus();
        }

        private string GetStatusText()
        {
            bool isInstalled = ContextMenuManager.IsInstalled();
            return isInstalled 
                ? LanguageManager.StatusInstalled 
                : LanguageManager.StatusNotInstalled;
        }

        private void UpdateStatus()
        {
            if (_statusLabel != null)
            {
                _statusLabel.Text = GetStatusText();
            }
        }

        private void InstallBtn_Click(object? sender, EventArgs e)
        {
            if (ContextMenuManager.Install())
            {
                MessageBox.Show(
                    LanguageManager.InstallSuccess,
                    LanguageManager.InstallSuccessTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                UpdateStatus();
            }
            else
            {
                MessageBox.Show(
                    LanguageManager.InstallError,
                    LanguageManager.InstallErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UninstallBtn_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                LanguageManager.UninstallConfirm,
                LanguageManager.UninstallConfirmTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (ContextMenuManager.Uninstall())
                {
                    MessageBox.Show(
                        LanguageManager.UninstallSuccess,
                        LanguageManager.UninstallSuccessTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    UpdateStatus();
                }
                else
                {
                    MessageBox.Show(
                        LanguageManager.UninstallError,
                        LanguageManager.UninstallErrorTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void HistoryBtn_Click(object? sender, EventArgs e)
        {
            using (var historyForm = new HistoryForm())
            {
                historyForm.ShowDialog(this);
            }
        }
    }
}
