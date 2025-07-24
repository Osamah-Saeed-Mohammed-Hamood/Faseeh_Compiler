using System;
using System.Drawing;
using System.Windows.Forms;
using Faseeh.IDE.Core.Models;
using Faseeh.IDE.Core.Services;
using Faseeh.Presentation.Controls;
using System.Diagnostics;
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



        private RichTextBox _outputBox; // لوحة الإخراج
        private SplitContainer _splitContainer; // لتقسيم الشاشة

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

            // إنشاء العناصر بالترتيب
            CreateMenuStrip();
            CreateToolStrip();
            CreateTextBox(); // إنشاء محرر الكود أولاً
            CreateOutputPanel(); // ثم إنشاء لوحة الإخراج ووضع المحرر بداخلها
            CreateStatusStrip();

            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
        }

        private void CreateMenuStrip()
        {
            _menuStrip = new MenuStrip { Dock = DockStyle.Top };

            // --- قائمة ملف ---
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

            // --- قائمة تحرير ---
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

            // --- قائمة تنسيق ---
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

            // --- قائمة عرض ---
            var viewMenu = new ToolStripMenuItem("عرض");
            var themeMenu = new ToolStripMenuItem("السمة");
            themeMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
        CreateMenuItem("واضحة", "Light", null, (s, e) => ApplyTheme(false)),
        CreateMenuItem("داكنة", "Dark", null, (s, e) => ApplyTheme(true))
            });
            viewMenu.DropDownItems.Add(themeMenu);

            // --- قائمة تشغيل ---
            var runMenu = new ToolStripMenuItem("تشغيل");
            runMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
        CreateMenuItem("تشغيل", "Run", Keys.F5, OnRun)
            });

            // ✅ السطر الصحيح: إضافة كل القوائم إلى شريط القوائم مرة واحدة
            _menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, formatMenu, viewMenu, runMenu });

            // إضافة شريط القوائم إلى النافذة
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


            _toolStrip.Items.Add(new ToolStripSeparator());
            AddToolButton("Run", "تشغيل (F5)", OnRun); // أضف هذا الزر
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
            // this.Controls.Add(_textBox); // <-- السطر محذوف أو معطل
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
        private void CreateOutputPanel()
        {
            // إنشاء الحاوية المقسمة
            _splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = this.Height - 250,
                FixedPanel = FixedPanel.Panel2,
            };

            // إنشاء لوحة الإخراج
            _outputBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                RightToLeft = RightToLeft.No,
            };

            // ✅ الخطوة الأهم: ضع محرر الكود في الجزء العلوي من الحاوية
            _splitContainer.Panel1.Controls.Add(_textBox);

            // وضع لوحة الإخراج في الجزء السفلي
            _splitContainer.Panel2.Controls.Add(_outputBox);

            // إضافة الحاوية إلى النافذة الرئيسية
            this.Controls.Add(_splitContainer);

            // جعل الحاوية خلف القوائم العلوية
            _splitContainer.BringToFront(); // استخدم BringToFront() لضمان تفاعلها مع القوائم بشكل صحيح
        }

        private void OnRun(object sender, EventArgs e)
        {
            // -------------------------------------------------------------------
            // !! تنبيه هام !!
            // يجب تغيير هذا المسار إلى المسار الصحيح للملف التنفيذي الخاص بمترجم "فصيح"
            // -------------------------------------------------------------------
            string compilerPath = @"FaseehCompiler.exe";

            // 1. مسح المخرجات السابقة
            _outputBox.Clear();
            _outputBox.ForeColor = _settings.DarkMode ? Color.White : Color.Black;
            _outputBox.Text = "جاري تنفيذ عملية الترجمة...\n" + new string('-', 50) + "\n";

            // 2. التحقق من وجود ملف المترجم
            if (!System.IO.File.Exists(compilerPath))
            {
                _outputBox.ForeColor = Color.Red;
                _outputBox.AppendText($"خطأ فادح: لم يتم العثور على المترجم في المسار المحدد.\nالمسار: {System.IO.Path.GetFullPath(compilerPath)}\n");
                _outputBox.AppendText("\nيرجى التأكد من وضع ملف المترجم (FaseehCompiler.exe) في نفس مجلد هذا البرنامج، أو قم بتحديث المسار في ملف MainForm.cs.");
                return;
            }

            try
            {
                // 3. حفظ الكود الحالي في ملف مؤقت
                string sourceCode = _textBox.Text;
                string tempFile = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".faseeh");
                System.IO.File.WriteAllText(tempFile, sourceCode, System.Text.Encoding.UTF8);

                // 4. إعداد وتشغيل عملية المترجم
                var processInfo = new ProcessStartInfo(compilerPath, $"\"{tempFile}\"")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8
                };

                using (var process = Process.Start(processInfo))
                {
                    // 5. قراءة المخرجات والأخطاء
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    // 6. عرض النتائج في لوحة الإخراج
                    if (!string.IsNullOrEmpty(error))
                    {
                        _outputBox.ForeColor = Color.Red;
                        _outputBox.AppendText("❌ حدث خطأ أثناء الترجمة:\n");
                        _outputBox.AppendText(error);
                    }
                    else
                    {
                        _outputBox.ForeColor = Color.LimeGreen;
                        _outputBox.AppendText("✅ تمت الترجمة بنجاح.\n\n");
                        _outputBox.AppendText("📜 المخرجات:\n");
                        _outputBox.AppendText(output);
                    }
                }

                // 7. حذف الملف المؤقت
                System.IO.File.Delete(tempFile);
            }
            catch (Exception ex)
            {
                _outputBox.ForeColor = Color.OrangeRed;
                _outputBox.AppendText($"\nحدث خطأ غير متوقع في بيئة التطوير:\n{ex.Message}");
            }
        }
    }
}