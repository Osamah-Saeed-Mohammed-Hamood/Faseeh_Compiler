using System;
using System.Drawing;
using System.Windows.Forms;
using Faseeh.IDE.Core.Models;
using Faseeh.IDE.Core.Services;
using Faseeh.Presentation.Controls;

namespace Faseeh.Presentation
{
    public class MainForm : Form
    {
        private  FileService _fileService;
        private  EditorService _editorService;
        private  ThemeService _themeService;

        private MenuStrip _menuStrip;
        private RichTextBoxEx _textBox;
        private ToolStrip _toolStrip;
        private StatusStrip _statusStrip;
        private EditorSettings _settings;

        public MainForm(EditorSettings settings)
        {
            _settings = settings;
            InitializeServices();
            InitializeComponents();
            ApplyTheme();
        }

        private void InitializeServices()
        {
            _fileService = new FileService();
            _editorService = new EditorService();
            _themeService = new ThemeService();
        }

        private void InitializeComponents()
        {
            // إعدادات النافذة الرئيسية
            this.Text = "محرر فصيح";
            this.Width = 1000;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Tahoma", 9);
           // this.Icon = Properties.Resources.AppIcon;

            CreateMenuStrip();
            CreateToolStrip();
            CreateTextBox();
            CreateStatusStrip();

            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void CreateMenuStrip()
        {
            _menuStrip = new MenuStrip { Dock = DockStyle.Top };

            // قائمة ملف
            var fileMenu = new ToolStripMenuItem("ملف");
            fileMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("جديد", "New", Keys.Control | Keys.N, OnNew),
                CreateMenuItem("فتح...", "Open", Keys.Control | Keys.O, OnOpen),
                CreateMenuItem("حفظ", "Save", Keys.Control | Keys.S, OnSave),
                CreateMenuItem("حفظ باسم...", "SaveAs", Keys.Control | Keys.Shift | Keys.S, OnSaveAs),
                new ToolStripSeparator(),
                CreateMenuItem("خروج", "Exit", Keys.Alt | Keys.F4, (s, e) => Close())
            });

            // قائمة تحرير
            var editMenu = new ToolStripMenuItem("تحرير");
            editMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("تراجع", "Undo", Keys.Control | Keys.Z, (s, e) => _textBox.Undo()),
                CreateMenuItem("إعادة", "Redo", Keys.Control | Keys.Y, (s, e) => _textBox.Redo()),
                new ToolStripSeparator(),
                CreateMenuItem("قص", "Cut", Keys.Control | Keys.X, (s, e) => _textBox.Cut()),
                CreateMenuItem("نسخ", "Copy", Keys.Control | Keys.C, (s, e) => _textBox.Copy()),
                CreateMenuItem("لصق", "Paste", Keys.Control | Keys.V, (s, e) => _textBox.Paste()),
                new ToolStripSeparator(),
                CreateMenuItem("تحديد الكل", "SelectAll", Keys.Control | Keys.A, (s, e) => _textBox.SelectAll())
            });

            // قائمة تنسيق
            var formatMenu = new ToolStripMenuItem("تنسيق");
            formatMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("خط...", "Font", null, OnChangeFont),
                CreateMenuItem("لون النص...", "TextColor", null, OnTextColor),
                CreateMenuItem("لون الخلفية...", "BackColor", null, OnBackColor),
                new ToolStripSeparator(),
                CreateMenuItem("عريض", "Bold", Keys.Control | Keys.B, OnBold),
                CreateMenuItem("مائل", "Italic", Keys.Control | Keys.I, OnItalic),
                CreateMenuItem("تحته خط", "Underline", Keys.Control | Keys.U, OnUnderline)
            });

            // قائمة عرض
            var viewMenu = new ToolStripMenuItem("عرض");
            var themeMenu = new ToolStripMenuItem("السمة");
            themeMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                CreateMenuItem("واضحة", "Light", null, (s, e) => ApplyTheme(false)),
                CreateMenuItem("داكنة", "Dark", null, (s, e) => ApplyTheme(true))
            });
            viewMenu.DropDownItems.Add(themeMenu);

            _menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, formatMenu, viewMenu });
            this.Controls.Add(_menuStrip);
        }

        private ToolStripMenuItem CreateMenuItem(string text, string imageName, Keys? shortcut, EventHandler handler)
        {
            var item = new ToolStripMenuItem(text);
            if (imageName != null)
            {
            //    item.Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
            }
            if (shortcut.HasValue)
            {
                item.ShortcutKeys = shortcut.Value;
            }
            item.Click += handler;
            return item;
        }

        private void CreateToolStrip()
        {
            _toolStrip = new ToolStrip { Dock = DockStyle.Top };

            AddToolButton("New", "جديد", OnNew);
            AddToolButton("Open", "فتح", OnOpen);
            AddToolButton("Save", "حفظ", OnSave);
            _toolStrip.Items.Add(new ToolStripSeparator());
            AddToolButton("Cut", "قص", (s, e) => _textBox.Cut());
            AddToolButton("Copy", "نسخ", (s, e) => _textBox.Copy());
            AddToolButton("Paste", "لصق", (s, e) => _textBox.Paste());
            _toolStrip.Items.Add(new ToolStripSeparator());
            AddToolButton("Bold", "عريض", OnBold);
            AddToolButton("Italic", "مائل", OnItalic);
            AddToolButton("Underline", "تحت خط", OnUnderline);

            this.Controls.Add(_toolStrip);
        }

        private void AddToolButton(string imageName, string toolTip, EventHandler handler)
        {
            var button = new ToolStripButton
            {
             //   Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName),
                ToolTipText = toolTip,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };
            button.Click += handler;
            _toolStrip.Items.Add(button);
        }

        private void CreateTextBox()
        {
            _textBox = new RichTextBoxEx
            {
                Dock = DockStyle.Fill,
                Font = _settings.Font,
                RightToLeft = RightToLeft.Yes,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.ForcedBoth,
                AcceptsTab = true
            };
            _textBox.TextChanged += TextBox_TextChanged;
            _textBox.SelectionChanged += TextBox_SelectionChanged;
            this.Controls.Add(_textBox);
        }

        private void CreateStatusStrip()
        {
            _statusStrip = new StatusStrip();

            var statusLabel = new ToolStripStatusLabel
            {
                Text = "جاهز",
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var positionLabel = new ToolStripStatusLabel
            {
                Text = "السطر: 1 | العمود: 1",
                TextAlign = ContentAlignment.MiddleRight
            };

            _statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel, positionLabel });
            this.Controls.Add(_statusStrip);
        }

        private void ApplyTheme(bool? darkMode = null)
        {
            if (darkMode.HasValue)
            {
                _settings.DarkMode = darkMode.Value;
                _themeService.SaveSettings(_settings);
            }

            _themeService.ApplyTheme(this, _settings, _menuStrip, _toolStrip, _statusStrip, _textBox);
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            _editorService.UpdateStatus(_textBox, _statusStrip.Items[0]);
        }

        private void TextBox_SelectionChanged(object sender, EventArgs e)
        {
            var pos = _textBox.GetPosition();
            _statusStrip.Items[1].Text = $"السطر: {pos.Line + 1} | العمود: {pos.Column + 1}";
        }

        private void OnNew(object sender, EventArgs e) => _fileService.NewFile(_textBox);
        private void OnOpen(object sender, EventArgs e) => _fileService.OpenFile(_textBox);
        private void OnSave(object sender, EventArgs e) => _fileService.SaveFile(_textBox);
        private void OnSaveAs(object sender, EventArgs e) => _fileService.SaveFileAs(_textBox);

        private void OnChangeFont(object sender, EventArgs e)
        {
            using (var fontDialog = new FontDialog())
            {
                fontDialog.Font = _textBox.Font;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    _textBox.Font = fontDialog.Font;
                    _settings.Font = fontDialog.Font;
                    _themeService.SaveSettings(_settings);
                }
            }
        }

        private void OnTextColor(object sender, EventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = _textBox.SelectionColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    _textBox.SelectionColor = colorDialog.Color;
                }
            }
        }

        private void OnBackColor(object sender, EventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = _textBox.SelectionBackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    _textBox.SelectionBackColor = colorDialog.Color;
                }
            }
        }

        private void OnBold(object sender, EventArgs e) => _editorService.ToggleFontStyle(_textBox, FontStyle.Bold);
        private void OnItalic(object sender, EventArgs e) => _editorService.ToggleFontStyle(_textBox, FontStyle.Italic);
        private void OnUnderline(object sender, EventArgs e) => _editorService.ToggleFontStyle(_textBox, FontStyle.Underline);

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S && !e.Shift)
            {
                OnSave(sender, e);
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.S)
            {
                OnSaveAs(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _themeService.SaveSettings(_settings);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }
    }
}