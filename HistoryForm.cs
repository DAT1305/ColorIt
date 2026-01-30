using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ColorIt
{
    public partial class HistoryForm : Form
    {
        private ListView? _listView;

        public HistoryForm()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void InitializeComponent()
        {
            this.Text = LanguageManager.HistoryTitle;
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(500, 300);
            this.BackColor = Color.White;

            // Title
            var titleLabel = new Label
            {
                Text = "📋 " + LanguageManager.HistoryTitle,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            this.Controls.Add(titleLabel);

            // ListView
            _listView = new ListView
            {
                Location = new Point(20, 55),
                Size = new Size(645, 380),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _listView.Columns.Add("Color", 60, HorizontalAlignment.Center);
            _listView.Columns.Add("Folder Path", 400, HorizontalAlignment.Left);
            _listView.Columns.Add("Date", 150, HorizontalAlignment.Center);

            _listView.MouseDoubleClick += ListView_MouseDoubleClick;
            
            // Context menu for list
            var contextMenu = new ContextMenuStrip();
            
            var openItem = new ToolStripMenuItem(LanguageManager.OpenFolder);
            openItem.Click += OpenFolder_Click;
            contextMenu.Items.Add(openItem);

            var resetItem = new ToolStripMenuItem(LanguageManager.ResetColor);
            resetItem.Click += ResetColor_Click;
            contextMenu.Items.Add(resetItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            var removeItem = new ToolStripMenuItem(LanguageManager.RemoveFromList);
            removeItem.Click += RemoveFromList_Click;
            contextMenu.Items.Add(removeItem);

            _listView.ContextMenuStrip = contextMenu;

            this.Controls.Add(_listView);
        }

        private void LoadHistory()
        {
            if (_listView == null) return;

            _listView.Items.Clear();
            var history = FolderHistoryManager.GetHistory();

            if (history.Count == 0)
            {
                var emptyLabel = new Label
                {
                    Text = LanguageManager.NoHistory,
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(200, 200)
                };
                emptyLabel.Name = "emptyLabel";
                this.Controls.Add(emptyLabel);
                return;
            }

            // Remove empty label if exists
            var existingLabel = this.Controls["emptyLabel"];
            if (existingLabel != null)
            {
                this.Controls.Remove(existingLabel);
            }

            foreach (var item in history)
            {
                var listItem = new ListViewItem();
                
                // Color column - we'll show a colored square
                listItem.Text = "■";
                try
                {
                    var color = ColorTranslator.FromHtml(item.ColorHex);
                    listItem.ForeColor = color;
                }
                catch
                {
                    listItem.ForeColor = Color.Gray;
                }

                listItem.SubItems.Add(item.Path);
                listItem.SubItems.Add(item.ColoredDate.ToString("yyyy-MM-dd HH:mm"));
                listItem.Tag = item;

                _listView.Items.Add(listItem);
            }
        }

        private void ListView_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            OpenSelectedFolder();
        }

        private void OpenFolder_Click(object? sender, EventArgs e)
        {
            OpenSelectedFolder();
        }

        private void OpenSelectedFolder()
        {
            if (_listView == null || _listView.SelectedItems.Count == 0) return;

            var item = _listView.SelectedItems[0].Tag as ColoredFolderInfo;
            if (item != null && System.IO.Directory.Exists(item.Path))
            {
                try
                {
                    Process.Start("explorer.exe", item.Path);
                }
                catch { }
            }
        }

        private void ResetColor_Click(object? sender, EventArgs e)
        {
            if (_listView == null || _listView.SelectedItems.Count == 0) return;

            var item = _listView.SelectedItems[0].Tag as ColoredFolderInfo;
            if (item != null)
            {
                try
                {
                    FolderColorizer.ResetFolderColor(item.Path);
                    LoadHistory();
                    MessageBox.Show(
                        LanguageManager.ColorResetSuccess,
                        LanguageManager.ColorAppliedTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
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

        private void RemoveFromList_Click(object? sender, EventArgs e)
        {
            if (_listView == null || _listView.SelectedItems.Count == 0) return;

            var item = _listView.SelectedItems[0].Tag as ColoredFolderInfo;
            if (item != null)
            {
                FolderHistoryManager.Remove(item.Path);
                LoadHistory();
            }
        }
    }
}
