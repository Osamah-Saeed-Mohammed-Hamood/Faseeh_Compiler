using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Faseeh.IDE.Core.Models;
using Faseeh.Presentation.Controls;

namespace Faseeh.IDE.Core.Services
{
    public class ThemeService
    {
        private const string SettingsFile = "settings.dat";

        public void ApplyTheme(Form form, EditorSettings settings,
                             MenuStrip menu, ToolStrip tool,
                             StatusStrip status, RichTextBoxEx textBox)
        {
            if (settings.DarkMode)
            {
                // الألوان الداكنة
                Color darkBack = Color.FromArgb(30, 30, 36);
                Color darkControl = Color.FromArgb(45, 45, 48);
                Color darkText = Color.FromArgb(220, 220, 220);
                Color darkHighlight = Color.FromArgb(51, 153, 255);

                form.BackColor = darkBack;

                menu.BackColor = darkControl;
                menu.ForeColor = darkText;

                tool.BackColor = darkControl;
                tool.ForeColor = darkText;

                status.BackColor = darkControl;
                status.ForeColor = darkText;

                textBox.BackColor = darkControl;
                textBox.ForeColor = darkText;
                textBox.SelectionColor = darkHighlight;

                foreach (ToolStripMenuItem item in menu.Items)
                {
                    ApplyDarkItem(item);
                }
            }
            else
            {
                // الألوان الفاتحة
                Color lightBack = Color.FromArgb(240, 240, 240);
                Color lightControl = Color.White;
                Color lightText = Color.FromArgb(30, 30, 30);
                Color lightHighlight = Color.DodgerBlue;

                form.BackColor = lightBack;

                menu.BackColor = lightBack;
                menu.ForeColor = lightText;

                tool.BackColor = lightBack;
                tool.ForeColor = lightText;

                status.BackColor = lightBack;
                status.ForeColor = Color.FromArgb(80, 80, 80);

                textBox.BackColor = lightControl;
                textBox.ForeColor = lightText;
                textBox.SelectionColor = lightHighlight;

                foreach (ToolStripMenuItem item in menu.Items)
                {
                    ApplyLightItem(item);
                }
            }

            textBox.Font = settings.Font;
        }

        private void ApplyDarkItem(ToolStripMenuItem item)
        {
            item.BackColor = Color.FromArgb(45, 45, 48);
            item.ForeColor = Color.FromArgb(220, 220, 220);

            foreach (ToolStripItem subItem in item.DropDownItems)
            {
                if (subItem is ToolStripMenuItem menuItem)
                {
                    ApplyDarkItem(menuItem);
                }
            }
        }

        private void ApplyLightItem(ToolStripMenuItem item)
        {
            item.BackColor = Color.FromArgb(240, 240, 240);
            item.ForeColor = Color.FromArgb(30, 30, 30);

            foreach (ToolStripItem subItem in item.DropDownItems)
            {
                if (subItem is ToolStripMenuItem menuItem)
                {
                    ApplyLightItem(menuItem);
                }
            }
        }

        public EditorSettings LoadSettings()
        {
            if (File.Exists(SettingsFile))
            {
                try
                {
                    using (var stream = File.OpenRead(SettingsFile))
                    {
                        var formatter = new BinaryFormatter();
                        return (EditorSettings)formatter.Deserialize(stream);
                    }
                }
                catch
                {
                    return new EditorSettings();
                }
            }
            return new EditorSettings();
        }

        public void SaveSettings(EditorSettings settings)
        {
            try
            {
                using (var stream = File.Create(SettingsFile))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, settings);
                }
            }
            catch { /* تجاهل الأخطاء في حفظ الإعدادات */ }
        }
    }
}